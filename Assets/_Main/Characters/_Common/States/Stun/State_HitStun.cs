using System.Collections;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnHitStunGround")]
public class State_CmnHitStun : State_Common_Stun {
    public State_CmnHitStun(Entity entity) : base(entity) { }
    protected override int frames => player.frameData.hitstunFrames;
    protected override string animationName => "std/hitstun_ground";
    public override EntityStateType type => EntityStateType.CHR_HITSTUN;

    protected override void OnStateBegin() {
        base.OnStateBegin();
        // player.ApplyGroundedFrictionImmediate();
        AddCancelOption("CmnHitStunAir");
    }

}

[NamedToken("CmnHitStunGroundCrouch")]
public class State_CmnHitStunCrouch : State_Common_Stun {
    public State_CmnHitStunCrouch(Entity entity) : base(entity) { }
    protected override int frames => player.frameData.hitstunFrames;
    protected override string animationName => "std/hitstun_ground_crouch";
    public override EntityStateType type => EntityStateType.CHR_HITSTUN;

    protected override void OnStateBegin() {
        base.OnStateBegin();
        // player.ApplyGroundedFrictionImmediate();
        AddCancelOption("CmnHitStunAir");
    }
}

[NamedToken("CmnHitStunAir")]
public class State_CmnHitStunAir : State_Common_Stun {
    public State_CmnHitStunAir(Entity entity) : base(entity) { }
    protected override int frames => player.frameData.hitstunFrames;
    protected override string animationName => "std/hitstun_air";
    public override EntityStateType type => EntityStateType.CHR_HITSTUN;
    public override bool mayEnterState => player.airborne;
    private bool landed = false;
    private LandingRecoveryFlag landingRecoveryFlag;
    
    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.airborne = true;
        stateData.targetFrameRate = 12;
        landed = false;
        landingRecoveryFlag = LandingRecoveryFlag.NONE;
    }

    public override EntityStateSubroutine BeginMainSubroutine() {
        return Sub_WaitForLand;
    }

    protected virtual void Sub_WaitForLand(SubroutineContext ctx) {
        if (!landed) {
            ctx.Repeat();
            return;
        }
        
        if (player.frameData.shouldGroundBounce) {
            var force = player.frameData.ConsumeContactBounce();
            
            var decayData = opponent.comboDecayData;
            var comboCounter = player.comboCounter;
            player.rb.AddForce(force.bounceForce * new Vector2(
                                   decayData.opponentBlowbackCurve.Evaluate(comboCounter.comboDecay), 
                                   decayData.opponentLaunchCurve.Evaluate(comboCounter.comboDecay)
                               ) * new Vector2(player.side == EntitySide.RIGHT ? 1 : -1, 1), ForceMode2D.Impulse); 
            // while (player.transform.position.y < 1f) {
            //     yield return 1;
            // }
            landed = false;
            player.airborne = true;
            
            player.airHitstunRotation = 0f;  
            if (force.flags.HasFlag(BounceFlags.HEAVY)) {
                player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/land/hard", CharacterFXSocketType.WORLD_UNBOUND, player.transform.position);
                entity.PlaySound("cmn/battle/sfx/wall_bounce");
                SimpleCameraShakePlayer.inst.PlayCommon("groundbounce_heavy");
                    
            } else {
                SimpleCameraShakePlayer.inst.PlayCommon("groundbounce");
            }

            player.frameData.forcedAirborneFrames = 5;
            ctx.Repeat();
            return;
        }
        
        CancelInto(landingRecoveryFlag.HasFlag(LandingRecoveryFlag.HARD_KNOCKDOWN_LAND) ? "CmnHardKnockdown" : "CmnSoftKnockdown");
    }

    public override void OnLand(LandingRecoveryFlag flag, int recoveryFrames) {
        base.OnLand(flag, recoveryFrames);
        landed = true;
        landingRecoveryFlag |= flag;
    }
}

}
