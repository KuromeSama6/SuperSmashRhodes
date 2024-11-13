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
    public State_Common_NmlAtk5CS(Entity owner) : base(owner) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_5CS;
    public override float inputPriority => 3.1f;

    protected override string mainAnimation => "cmn_NmlAtk5CS";

    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_SPECIAL_SUPER | 
                                                             EntityStateType.CHR_ATK_5S | EntityStateType.CHR_ATK_2S
                                                                                         | EntityStateType.CHR_ATK_6P
                                                                                         | EntityStateType.CHR_ATK_5H | EntityStateType.CHR_ATK_6H | EntityStateType.CHR_ATK_2H
                                                                                         | EntityStateType.CHR_ATK_5D | EntityStateType.CHR_ATK_2D;
    public override bool mayEnterState => player.opponentDistance <= triggerRange;

    protected override InputFrame[] requiredInput => new InputFrame[] {new(InputType.S, InputFrameType.PRESSED)};

    public override int GetFreezeFrames(Entity to) {
        return 9;
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
    public override Vector2 GetPushback(Entity to, bool airborne) {
        return airborne ? new Vector2(1.5f, 5f) : new Vector2(3f, 0);
    }
    public override int GetAttackLevel(Entity to) {
        return 3;
    }

    protected virtual float triggerRange => 0.8f;
}
}
