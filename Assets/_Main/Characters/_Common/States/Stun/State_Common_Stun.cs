using System.Collections;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_Stun : CharacterState {
    protected State_Common_Stun(Entity entity) : base(entity) { }
    public override float inputPriority => -1;
    public override bool mayEnterState => false;
    public override bool IsInputValid(InputBuffer buffer) {
        return false;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        entity.animation.ApplyNeutralPose();
        entity.animation.AddUnmanagedAnimation(animationName, true, 0);
        
        AddCancelOption("CmnBurst");
        stateData.maySwitchSides = true;
    }

    public override EntityStateSubroutine BeginMainSubroutine() {
        return Sub_StunLoop;
    }

    protected virtual void Sub_StunLoop(SubroutineContext ctx) {
        if (frames > 0) {
            player.ApplyGroundedFriction();

            if (!player.airborne && player.transform.position.y >= 0.2f) {
                CancelInto("CmnHitStunAir");
            }
            
            ctx.Repeat();
            return;
        }
        
        ctx.Exit();
    }
    
    protected override void OnStateEnd(EntityState nextState) {
        base.OnStateEnd(nextState);
        player.frameData.throwInvulnFrames = 5;
    }

    // Abstract properties
    protected abstract int frames { get; }
    protected abstract string animationName { get; }
}
}
