using System.Collections;
using SuperSmashRhodes.Battle;
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

    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.airborne = true;
    }

    public override IEnumerator MainRoutine() {
        while (true) yield return 1;
    }

    public override void OnLand(LandingRecoveryFlag flag, int recoveryFrames) {
        base.OnLand(flag, recoveryFrames);
        CancelInto(flag.HasFlag(LandingRecoveryFlag.HARD_KNOCKDOWN_LAND) ? "CmnHardKnockdown" : "CmnSoftKnockdown");
    }
}

}
