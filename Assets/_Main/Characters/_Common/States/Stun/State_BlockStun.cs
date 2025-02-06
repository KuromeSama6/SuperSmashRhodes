using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {

[NamedToken("CmnBlockStun")]
public class State_CmnBlockStun : State_Common_Stun {
    public State_CmnBlockStun(Entity owner) : base(owner) { }
    protected override int frames => player.frameData.blockstunFrames;
    protected override string animationName => "std/blockstun";
    public override EntityStateType type => EntityStateType.CHR_BLOCKSTUN;
    public override bool mayEnterState => owner.activeState is State_CmnBlockStunCrouch;
    public override bool IsInputValid(InputBuffer buffer) {
        return !buffer.thisFrame.HasInput(owner.side, InputType.DOWN, InputFrameType.HELD);
    }
    protected override void OnStateBegin() {
        base.OnStateBegin();
        AddCancelOption("CmnBlockStunCrouch");
        // player.ApplyGroundedFrictionImmediate();
    }
}

[NamedToken("CmnBlockStunCrouch")]
public class State_CmnBlockStunCrouch : State_Common_Stun {
    public State_CmnBlockStunCrouch(Entity owner) : base(owner) { }
    protected override int frames => player.frameData.blockstunFrames;
    protected override string animationName => "std/blockstun_crouch";
    public override EntityStateType type => EntityStateType.CHR_BLOCKSTUN;

    public override bool mayEnterState => owner.activeState is State_CmnBlockStun;
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.thisFrame.HasInput(owner.side, InputType.DOWN, InputFrameType.HELD);
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        AddCancelOption("CmnBlockStun");
        // player.ApplyGroundedFrictionImmediate();
    }

    public override IEnumerator MainRoutine() {
        while (frames > 0) {
            player.ApplyGroundedFriction();
            if (!RevalidateInput()) {
                CancelInto("CmnBlockStun");
                yield break;
            }
            yield return 1;
        }
    }

}
}
