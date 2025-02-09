using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine.AddressableAssets;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnHardKnockdown")]
public class State_CmnHardKnockdown : CharacterState {
    public State_CmnHardKnockdown(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_HARD_KNOCKDOWN;
    public override float inputPriority => -1;

    public override bool IsInputValid(InputBuffer buffer) {
        return false;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        entity.animation.AddUnmanagedAnimation("std/down", false);
    }
    public override IEnumerator MainRoutine() {
        yield return 55;
        CancelInto("CmnSoftKnockdown");
    }

    protected override void OnStateEnd() {
        base.OnStateEnd();
        player.frameData.throwInvulnFrames = 5;
    }
}
}
