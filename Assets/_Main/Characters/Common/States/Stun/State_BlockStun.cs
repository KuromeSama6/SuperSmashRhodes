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
    protected override string animationName => "std_blockstun";
    public override EntityStateType type => EntityStateType.CHR_BLOCKSTUN;
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
    public override EntityStateType type => EntityStateType.CHR_BLOCKSTUN;
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.thisFrame.HasInput(InputType.DOWN, InputFrameType.HELD);
    }

    public override IEnumerator MainRoutine() {
        while (frames > 0) {
            if (!RevalidateInput()) {
                CancelInto("CmnBlockStun");
                player.ApplyGroundedFriction();
                yield break;
            }
            yield return 1;
        }
    }

}
}
