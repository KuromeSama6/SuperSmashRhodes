using System.Collections.Generic;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State {
public class EntityStateData {
    // Cancel options
    /// <summary>
    /// States that can be canceled into from this state.
    /// </summary>
    public List<EntityState> cancelOptions { get; } = new List<EntityState>();

    /// <summary>
    /// A flag that determines if the current state can be cancelled into a specific type of states.
    /// </summary>
    public EntityStateType cancelFlag;
    public Dictionary<string, object> carriedVariables { get; } = new();
    public bool disableSideSwap = false;

    public void ClearCancelOptions() {
        cancelFlag = 0;
        cancelOptions.Clear();
    }
    
    public T GetCarriedVariable<T>(string key, T def = default) {
        if (carriedVariables.TryGetValue(key, out var value)) {
            if (!(value is T)) {
                Debug.LogError($"Carried variable [{key}] type mismatch, expected {typeof(T)}, got {value.GetType()}");
                return def;
            }
            return (T)value;
        }
        
        Debug.LogError($"Carried variable [{key}] not found");
        return def;
    }
    
    public bool TryGetCarriedVariable<T>(string key, out T value) {
        if (carriedVariables.TryGetValue(key, out var obj)) {
            if (obj is T t) {
                value = t;
                return true;
            }
        }
        value = default;
        return false;
    }
}
}
