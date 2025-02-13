using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnSoftKnockdown")]
public class State_CmnSoftKnockdown : CharacterState {
    public State_CmnSoftKnockdown(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_SOFT_KNOCKDOWN;
    public override float inputPriority => -1;
    public override AttackType invincibility => AttackType.FULL;

    public override bool IsInputValid(InputBuffer buffer) {
        return false;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        entity.animation.AddUnmanagedAnimation("std/ground_tech", false);
    }
    public override IEnumerator MainRoutine() {
        player.fxManager.staticOnGroundedTechFlashPlayer.PlayFeedbacks();
        yield return 1;
        player.comboCounter.Reset();
        yield return 30;
        player.neutralAniTransitionOverride = 0f;
    }

    protected override void OnStateEnd(string nextState) {
        base.OnStateEnd(nextState);
        player.frameData.throwInvulnFrames = 5;
    }
}
}
