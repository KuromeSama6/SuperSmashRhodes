using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_Stun : CharacterState {
    protected State_Common_Stun(Entity owner) : base(owner) { }
    public override EntityStateType type => EntityStateType.CHR_STUN;
    public override int inputPriority => -1;
    public override bool mayEnterState => false;
    public override bool IsInputValid(InputBuffer buffer) {
        return false;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        owner.animation.AddUnmanagedAnimation(animationName, true);
    }

    public override IEnumerator MainRoutine() {
        while (frames > 0) {
            if (!player.airborne) player.ApplyGroundedFriction();
            yield return 1;
        }
    }

    // Abstract properties
    protected abstract int frames { get; }
    protected abstract string animationName { get; }
}

[NamedToken("CmnBlockStun")]
public class State_CmnBlockStun : State_Common_Stun {
    public State_CmnBlockStun(Entity owner) : base(owner) { }
    protected override int frames => player.frameData.blockstunFrames;
    protected override string animationName => "std_blockstun";
    protected override void OnStateBegin() {
        base.OnStateBegin();
        AddCancelOption("CmnBlockStunCrouch");
    }
}

[NamedToken("CmnBlockStunCrouch")]
public class State_CmnBlockStunCrouch : State_Common_Stun {
    public State_CmnBlockStunCrouch(Entity owner) : base(owner) { }
    protected override int frames => player.frameData.blockstunFrames;
    protected override string animationName => "std_blockstun_crouch";
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.thisFrame.HasInput(InputType.DOWN, InputFrameType.HELD);
    }

    public override IEnumerator MainRoutine() {
        while (frames > 0) {
            if (!RevalidateInput()) {
                CancelInto("CmnBlockStun");
                yield break;
            }
            yield return 1;
        }
    }

}
}
