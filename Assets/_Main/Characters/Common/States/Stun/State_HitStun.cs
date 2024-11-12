using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnHitStunGround")]
public class State_HitStun : State_Common_Stun {
    public State_HitStun(Entity owner) : base(owner) { }
    protected override int frames => player.frameData.hitstunFrames;
    protected override string animationName => "std_hitstun_ground";
    public override EntityStateType type => EntityStateType.CHR_HITSTUN;

    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.ApplyGroundedFrictionImmediate();
    }

}

[NamedToken("CmnHitStunGroundCrouch")]
public class State_HitStunCrouch : State_Common_Stun {
    public State_HitStunCrouch(Entity owner) : base(owner) { }
    protected override int frames => player.frameData.hitstunFrames;
    protected override string animationName => "std_hitstun_ground_crouch";
    public override EntityStateType type => EntityStateType.CHR_HITSTUN;

    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.ApplyGroundedFrictionImmediate();
    }
}

}
