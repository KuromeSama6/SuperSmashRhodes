using System;
using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_NmlAtk2H : State_Common_NormalAttack {
    public State_Common_NmlAtk2H(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_2H;
    public override float inputPriority => 4;

    protected override string mainAnimation => "cmn/NmlAtk2H";

    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_DRIVE_SPECIAL_SUPER;
    protected override InputFrame[] requiredInput => new InputFrame[] {new(InputType.DOWN, InputFrameType.HELD), new(InputType.HS, InputFrameType.PRESSED)};

    public override int GetFreezeFrames(Entity to) {
        return 8;
    }
    public override float GetComboProration(Entity to) {
        return .9f;
    }
    public override float GetFirstHitProration(Entity to) {
        return .95f;
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (airborne) return new Vector2(2.5f, 5f);
        return blocked ? new(4f, 0f) : new Vector2(.5f, 9f);
    }
    public override int GetAttackLevel(Entity to) {
        return 4;
    }
    public override Vector2 GetCarriedMomentumPercentage(Entity to) {
        return Vector2.zero;
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.LARGE;
    }
}
}
