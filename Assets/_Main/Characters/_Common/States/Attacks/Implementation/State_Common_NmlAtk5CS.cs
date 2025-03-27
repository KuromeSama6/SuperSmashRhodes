using System;
using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_NmlAtk5CS : State_Common_NormalAttack {
    public State_Common_NmlAtk5CS(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_5CS;
    public override float inputPriority => 3.2f;

    protected override string mainAnimation => "cmn/NmlAtk5CS";

    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_NORMAL | EntityStateType.CHR_ATK_DRIVE_SPECIAL_SUPER;
    public override bool mayEnterState => player.opponentDistance <= triggerRange && !GetCurrentInputBuffer().thisFrame.HasInput(entity.side, InputType.DOWN, InputFrameType.HELD) && base.mayEnterState;

    protected override InputFrame[] requiredInput => new InputFrame[] {new(InputType.S, InputFrameType.PRESSED)};
    public override void OnContact(Entity to) {
        base.OnContact(to);
        AddCancelOption("CmnJump");
    }
    public override float GetComboProration(Entity to) {
        return .99f;
    }
    public override float GetFirstHitProration(Entity to) {
        return 1f;
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override int GetAttackLevel(Entity to) {
        return 3;
    }
    public override Vector2 GetCarriedMomentumPercentage(Entity to) {
        return new(.5f, .2f);
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.MEDIUM;
    }
    protected virtual float triggerRange => 0.8f;
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (airborne) return new Vector2(1.5f, blocked ? 5f : 7f);
        return new(blocked ? 3f : 2f, 0f);
    }
}
}
