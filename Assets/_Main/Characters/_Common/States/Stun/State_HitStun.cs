using System.Collections;
using SuperSmashRhodes.Battle;
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

    public override IEnumerator MainRoutine() {
        while (true) {
            while (!landed) {
                yield return 1;
            }

            if (player.frameData.shouldGroundBounce) {
                var force = player.frameData.ConsumeContactBounce();
                var decayData = opponent.comboDecayData;
                var comboCounter = player.comboCounter;
                player.rb.AddForce(force * new Vector2(
                                       decayData.opponentBlowbackCurve.Evaluate(comboCounter.comboDecay), 
                                       decayData.opponentLaunchCurve.Evaluate(comboCounter.comboDecay)
                                   ), ForceMode2D.Impulse); 
                // while (player.transform.position.y < 1f) {
                //     yield return 1;
                // }
                landed = false;
                player.airborne = true;
                continue;
            }
            
            break;
        }
        CancelInto(landingRecoveryFlag.HasFlag(LandingRecoveryFlag.HARD_KNOCKDOWN_LAND) ? "CmnHardKnockdown" : "CmnSoftKnockdown");
    }

    public override void OnLand(LandingRecoveryFlag flag, int recoveryFrames) {
        base.OnLand(flag, recoveryFrames);
        landed = true;
        landingRecoveryFlag = flag;
    }
}

}
