using System;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.State;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.Tokens {
public class Token_Rosmontis_ThrownSword : Projectile, IEngineUpdateListener {
    private string defaultState;
    
    public override void Init() {
        base.Init();
    }

    public void InitSword(string defaultState) {
        if (this.defaultState != null)
            throw new InvalidOperationException("Sword already initialized");
        
        this.defaultState = defaultState;
    }
    
    public override EntityState GetDefaultState() {
        return states[defaultState];
    }

}
}
