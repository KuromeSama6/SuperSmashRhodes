using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Network.Room;
using UnityEditor.Tilemaps;
using UnityEngine;

namespace SuperSmashRhodes.Battle.Game {
public class GameStateManager : SingletonBehaviour<GameStateManager> {
    [Title("References")]
    public RoomConfiguration config;
    
    public int frame { get; private set; }
    public bool requiresStateCache => config.isNetworked || config.cacheGameState;

    private readonly List<IManualUpdate> manualUpdates = new();
    private readonly Dictionary<int, IAutoSerialize> autoSerializers = new();
    private readonly Dictionary<int, SerializedGameState> serializedGameStates = new();
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
                m.ManualUpdate();
            }
        });
    }

    private void FixedUpdate() {
        // load queued states
        while (queuedStateLoads.Count > 0) {
            LoadGameStateImmediate(queuedStateLoads.Dequeue());
        }
        
        manualUpdates.ForEach(m => {
            if (m is MonoBehaviour beh && beh && beh.isActiveAndEnabled) {
                // Debug.Log(m);
                m.ManualFixedUpdate();
            }
        });
        
        frame++;

        if (queuedStateSaves.Count > 0) {
            var state = SerializeGameStateImmediate();
            while (queuedStateSaves.Count > 0) {
                queuedStateSaves.Dequeue().Invoke(state);
            }
        }
    }

    public void QueueLoadGameState(SerializedGameState state) {
        queuedStateLoads.Enqueue(state);
    }
    
    public void QueueSaveGameState(Action<SerializedGameState> saveAction) {
        queuedStateSaves.Enqueue(saveAction);
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
