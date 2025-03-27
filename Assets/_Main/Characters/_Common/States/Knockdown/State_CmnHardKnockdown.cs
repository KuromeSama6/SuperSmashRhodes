using System.Collections;
using System.Collections.Generic;
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
    public override EntityStateSubroutine BeginMainSubroutine() {
        return ctx => ctx.Exit(25);
    }

    protected override void OnStateEndComplete(EntityState nextState) {
        base.OnStateEndComplete(nextState);
        player.SetCarriedStateVariable("_hardKnockdown", "CmnSoftKnockdown", true);
        CancelInto("CmnSoftKnockdown");
    }

    protected override void OnStateEnd(EntityState nextState) {
        base.OnStateEnd(nextState);
        player.frameData.throwInvulnFrames = 5;
    }
}
}
