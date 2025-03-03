using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Network;
using SuperSmashRhodes.Network.RoomManagement;
using UnityEngine;

namespace SuperSmashRhodes.Battle.Game {
public class GameStateManager : SingletonBehaviour<GameStateManager> {
    [Title("References")]
    public RoomConfiguration config;
    
    public int frame { get; private set; }
    public bool requiresStateCache => networkInputManager != null && networkInputManager.shouldCacheGameState;
    public NetworkInputManager networkInputManager => RoomManager.current is NetworkRoom networkRoom ? networkRoom.inputManager : null;

    private readonly List<IManualUpdate> manualUpdates = new();
    private readonly Dictionary<int, IAutoSerialize> autoSerializers = new();
    private readonly Dictionary<int, SerializedGameState> gamestateCache = new();
    private readonly Queue<SerializedGameState> queuedStateLoads = new();
    private readonly Queue<Action<SerializedGameState>> queuedStateSaves = new();
    
    
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

    /**
     * Central update loop for all manual updates
     * Each logical frame consists of the following steps:
     * 0. Check if there is a NetworkInputManager present; if so, pause game updates if the network input managers requires it.
     * 1. Loads any queued game states to the current state.
     * 2. Increments the frame counter. This marks the beginning of a new frame.
     * 3. Calls LogicPreUpdate on all manual updates.
     * 4. Saves the current game state if the configuration requires it.
     * 5. Ticks the NetworkInputManager if present.
     * 6. Calls LogicUpdate on all manual updates.
     * 7. Satifies any queued state saves by external requests.
     */
    private void FixedUpdate() {
        // 0. check network input manager
        if (networkInputManager != null) {
            if (networkInputManager.pauseGameState) return;
        }
        
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
                    m.LogicPreUpdate();
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
        });

        // 4. save state
        if (requiresStateCache) {
            var gameState = SerializeGameStateImmediate();
            gamestateCache[frame] = gameState;

            if (gamestateCache.Count > 120) {
                Debug.LogWarning($"Game state cache is too big (at {gamestateCache.Count}). Continuing may lead to performance issues and memory leaks.");
            }
        }
        
        // 5. tick network input manager
        networkInputManager?.Tick();
        
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
                    m.LogicUpdate();
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
        });
    }

    public void RollbackToFrame(int targetFrame) {
        if (gamestateCache.ContainsKey(targetFrame)) {
            LoadGameStateImmediate(gamestateCache[targetFrame]);
            gamestateCache.Remove(targetFrame);
        } else {
            throw new InvalidOperationException($"Cannot rollback to frame {targetFrame} as it is not cached.");
        }
    }
    
    public void DeleteGameState(int frame) {
        gamestateCache.Remove(frame);
    }
    
    public void QueueLoadGameState(SerializedGameState state) {
        queuedStateLoads.Enqueue(state);
    }
    
    public void QueueSaveGameState(Action<SerializedGameState> saveAction) {
        queuedStateSaves.Enqueue(saveAction);
    }

    public void ResetFrame() {
        frame = 0;
    }
    
    private SerializedGameState SerializeGameStateImmediate() {
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
    
    private void LoadGameStateImmediate(SerializedGameState state) {
        var ser = state.serializer;

        {
            // auto serializers
            var autoSerializers = ser.GetObject("autoSerializers");
            foreach (var (id, pth) in autoSerializers.objects) {
                this.autoSerializers[int.Parse(id)].Deserialize(autoSerializers.GetObject(id));
            }
        }
        
    }
    
}

public interface IAutoSerialize : IStateSerializable {
    int GetInstanceID();
}
}
