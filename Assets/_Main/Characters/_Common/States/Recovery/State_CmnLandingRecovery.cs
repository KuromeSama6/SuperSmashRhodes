﻿using System.Collections;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnLandingRecovery")]
public class State_CmnLandingRecovery : CharacterState {
    public State_CmnLandingRecovery(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_COMMON_RECOVERY;
    public override float inputPriority { get; }
    public override Hitstate hitstate => recoverInCounterhitState ? Hitstate.COUNTER : Hitstate.PUNISH;

    private bool recoverInCounterhitState;
    
    public override bool IsInputValid(InputBuffer buffer) {
        return false;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        
        recoverInCounterhitState = stateData.TryGetCarriedVariable("_landingRecoveryCounterHitState", out bool value) && value;
        
        var animationName = stateData.carriedLandingAnimation ?? "std/land";
        // Debug.Log(player.frameData.landingRecoveryFrames);
        
        if (player.frameData.landingRecoveryFrames <= 0) {
            player.frameData.landingRecoveryFrames = 6;
        }
        
        player.animation.AddUnmanagedAnimation(animationName, false);
    }
    
    public override EntityStateSubroutine BeginMainSubroutine() {
        return Sub_Loop;
    }

    protected virtual void Sub_Loop(SubroutineContext ctx) {
        if (player.frameData.landingRecoveryFrames > 0) {
            --player.frameData.landingRecoveryFrames;
            ctx.Repeat();
            return;
        }
        
        ctx.Exit();
    }
    
    protected override void OnTick() {
        base.OnTick();
        if (frame == 2) {
            AddCancelOption(EntityStateType.CHR_ATK_SYSTEMSPECIAL | EntityStateType.CHR_DRIVE_RELEASE);

            if (stateData.TryGetCarriedVariable<EntityStateType>("_carriedCancelFlags", out var flag)) {
                // Debug.Log(flag);
                AddCancelOption(flag);
            } 
        }
    }

}
}
