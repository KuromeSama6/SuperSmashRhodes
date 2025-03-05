using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.GGPOWrapper;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Network;
using SuperSmashRhodes.Network.RoomManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SuperSmashRhodes.Battle.Game {
public class FightEngine : AutoInitSingletonBehaviour<FightEngine> {
    public int frame { get; private set; }
    public GGPOConnector ggpo => RoomManager.current is NetworkRoom networkRoom ? networkRoom.ggpo : null;
    public Room room => RoomManager.current;
    public bool inRoom => room != null;
    
    private readonly List<IManualUpdate> manualUpdates = new();
    private readonly Dictionary<int, IAutoSerialize> autoSerializers = new();
    private readonly Queue<SerializedGameState> queuedStateLoads = new();
    private readonly Queue<Action<SerializedGameState>> queuedStateSaves = new();

    protected override void Awake() {
        base.Awake();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void Start() {
        // scan manual updates
        // Debug.Log(manualUpdates.Count);
        
        RefreshComponentReferences();
    }

    public void RefreshComponentReferences() {
        foreach (var beh in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)) {
            if (beh is IManualUpdate manualUpdate && !manualUpdates.Contains(manualUpdate)) {
                manualUpdates.Add(manualUpdate);
            }
            
            if (beh is IAutoSerialize autoSerialize) {
                autoSerializers[autoSerialize.GetInstanceID()] = autoSerialize;
            }
        }
        
    }
    
    private void Update() {
        manualUpdates.RemoveAll(c => c is MonoBehaviour beh && !beh);
        manualUpdates.ForEach(m => {
            if (m is MonoBehaviour beh && beh.isActiveAndEnabled) {
                try {
                    m.ManualUpdate();
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
        });
    }
    
    private void FixedUpdate() {
        if (ggpo != null && room != null) {
            TickGameStateGGPO();
        } else {
            TickGameStateLocal();
        }
        
    }

    private void TickGameStateGGPO() {
        if (ggpo == null) return;
        var networkRoom = (NetworkRoom)room;
        Debug.Log("tick ggpo");
        
        // poll inputs
        manualUpdates.ForEach(m => {
            if (m is MonoBehaviour beh && beh && beh.isActiveAndEnabled) {
                // Debug.Log(m);
                try {
                    m.EnginePreUpdate();
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
        });
        
        // add local input
        {
            var localInput = networkRoom.localThisFrameInputs;
            byte[] data = localInput != null ? localInput.Serialize() : new byte[0];
            var res = ggpo.AddLocalInputSync(networkRoom.localPlayer.playerId, data);
            if (res != GGPOStatusCode.OK) {
                Debug.LogError($"[FightEngine/GGPO] Failed to add local input: {res}");
            }
        }
        
        // receive remote inputs
        {
            var data = ggpo.GetRemoteInputSync(out var result);
            if (result == GGPOStatusCode.OK) {
                var remoteInput = new InputChord(data);
                networkRoom.remoteBuffer.inputBuffer.PushAndTick(remoteInput);
                
            } else {
                networkRoom.remoteBuffer.inputBuffer.PushAndTick(new InputChord());
                Debug.LogError($"FightEngine/GGPO] Failed to get remote input.");
            }
        }
        
        // advance
        manualUpdates.ForEach(m => {
            if (m is MonoBehaviour beh && beh && beh.isActiveAndEnabled) {
                // Debug.Log(m);
                try {
                    m.EnginePreUpdate();
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
        });
        
        // notify
        {
            var result = ggpo.NotifyTickSync();
            if (result != GGPOStatusCode.OK) {
                Debug.LogError($"[FightEngine/GGPO] Frame update failed!!!! {result}");
            }   
        }
        
        // idling is the last thing of the frame
        ggpo.NotifyIdleSyncNonBlocking(Time.fixedDeltaTime);
    }
    
    private void TickGameStateLocal() {
        // 0. check network input manager
        // if (ggpo != null) { //TODO Put back
        //     ggpo.PreTick();
        //     if (ggpo.pauseGameState) return;
        // }
        
        // 1. load queued states
        while (queuedStateLoads.Count > 0) {
            LoadGameStateImmediate(queuedStateLoads.Dequeue());
        }
        
        // 2. increment frame
        frame++;
        
        // 3. pre update
        manualUpdates.ForEach(m => {
            if (m is MonoBehaviour beh && beh && beh.isActiveAndEnabled) {
                // Debug.Log(m);
                try {
                    m.EnginePreUpdate();
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
        });
        
        // 6. update
        TickGameStateImmediate();
        
        // 7. satisfy queued state saves
        if (queuedStateSaves.Count > 0) {
            var state = SerializeGameStateImmediate();
            while (queuedStateSaves.Count > 0) {
                queuedStateSaves.Dequeue().Invoke(state);
            }
        }
    }
    
    public void TickGameStateImmediate() {
        manualUpdates.ForEach(m => {
            if (m is MonoBehaviour beh && beh && beh.isActiveAndEnabled) {
                // Debug.Log(m);
                try {
                    m.EngineUpdate();
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
        });
    }
    
    public void QueueLoadGameState(SerializedGameState state) {
        queuedStateLoads.Enqueue(state);
    }
    
    public void QueueSaveGameState(Action<SerializedGameState> saveAction) {
        queuedStateSaves.Enqueue(saveAction);
    }
    
    public SerializedGameState SerializeGameStateImmediate() {
        var ser = new StateSerializer(GetType());
        ser.Put("frame", frame);
        
        {
            // auto serializers
            var autoSerializers = new StateSerializer();
            foreach (var (id, autoSerialize) in this.autoSerializers) {
                var pth = new StateSerializer();
                pth.Put("_type", autoSerialize.GetType().FullName);
                autoSerialize.Serialize(pth);
                
                autoSerializers.Put(id.ToString(), pth.objects);
            }
            
            ser.Put("autoSerializers", autoSerializers.objects);
        }
        
        return new SerializedGameState(frame, ser);
    }
    
    public void LoadGameStateImmediate(SerializedGameState state) {
        var ser = state.serializer;

        {
            // auto serializers
            var autoSerializers = ser.GetObject("autoSerializers");
            foreach (var (id, pth) in autoSerializers.objects) {
                this.autoSerializers[int.Parse(id)].Deserialize(autoSerializers.GetObject(id));
            }
        }
        
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log("Scene loaded. Refreshing component references.");
        RefreshComponentReferences();
    }
    
    private void OnSceneUnloaded(Scene scene) {
        autoSerializers.Clear();
    }
    
}

public interface IAutoSerialize : IStateSerializable {
    int GetInstanceID();
}
}
