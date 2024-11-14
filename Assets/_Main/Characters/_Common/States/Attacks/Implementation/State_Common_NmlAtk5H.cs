using System;
using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_NmlAtk5H : State_Common_NormalAttack {
    public State_Common_NmlAtk5H(Entity owner) : base(owner) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_5H;
    public override float inputPriority => 3;

    protected override string mainAnimation => "cmn_NmlAtk5H";

    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_SPECIAL_SUPER;
    protected override InputFrame[] requiredInput => new InputFrame[] {new(InputType.HS, InputFrameType.PRESSED)};

    public override int GetFreezeFrames(Entity to) {
        return 8;
    }
    public override float GetComboProration(Entity to) {
        return .9f;
    }
    public override float GetFirstHitProration(Entity to) {
        return .9f;
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override Vector2 GetPushback(Entity to, bool airborne) {
        return airborne ? new Vector2(4f, 4f) : new Vector2(3.5f, 0);
    }
    public override int GetAttackLevel(Entity to) {
        return 4;
    }
}
}
