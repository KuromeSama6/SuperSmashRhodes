using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Game;
using UnityEngine;

namespace SuperSmashRhodes.UI {
public abstract class PerSideUIElement<T> : MonoBehaviour where T: MonoBehaviour {
    private Dictionary<int, T> instances = new();
    protected PlayerCharacter player => GameManager.inst.GetPlayer(playerIndex);
    
    [Title("PerSideUIElement References")]
    public int playerIndex;

    private void Awake() {
        if (instances.ContainsKey(playerIndex))
            throw new Exception("PerSideUIElement already has an instance for player index " + playerIndex);
        
        instances[playerIndex] = GetComponent<T>();
    }

    
    
    public T this[int index] {
        get {
            if (!instances.ContainsKey(index))
                throw new Exception("PerSideUIElement does not have an instance for player index " + index);

            return instances[index];
        }
    }

}
}
