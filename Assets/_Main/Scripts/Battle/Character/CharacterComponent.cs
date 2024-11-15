using System;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public abstract class CharacterComponent : MonoBehaviour {
    protected PlayerCharacter player { get; private set; }
    private void Awake() {
        player = GetComponent<PlayerCharacter>();
        player.onRoundInit.AddListener(OnRoundInit);
    }

    public virtual void OnRoundInit() {
        
    }
}
}
