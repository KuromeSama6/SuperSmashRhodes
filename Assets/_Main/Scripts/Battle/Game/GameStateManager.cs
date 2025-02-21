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
    private readonly Dictionary<int, SerializedGameState> serializedGameStates = new();
    
    private void Start() {
        // scan manual updates
        // Debug.Log(manualUpdates.Count);
        RefreshManualUpdate();
    }

    public void RefreshManualUpdate() {
        FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IManualUpdate>()
            .ForEach(m => {
                if (!manualUpdates.Contains(m)) {
                    manualUpdates.Add(m);
                }
            });
    }
    
    private void Update() {
        manualUpdates.ForEach(m => {
            if (m is MonoBehaviour beh && beh.isActiveAndEnabled) {
                m.ManualUpdate();
            }
        });
    }

    private void FixedUpdate() {
        manualUpdates.ForEach(m => {
            if (m is MonoBehaviour beh && beh.isActiveAndEnabled) {
                // Debug.Log(m);
                m.ManualFixedUpdate();
            }
        });
    }

}
}
