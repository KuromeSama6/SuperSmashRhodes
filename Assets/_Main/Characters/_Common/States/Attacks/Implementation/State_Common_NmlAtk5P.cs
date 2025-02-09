using System;
using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_NmlAtk5P : State_Common_NormalAttack {
    public State_Common_NmlAtk5P(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_5P;
    public override float inputPriority => 3;

    protected override string mainAnimation => "cmn/NmlAtk5P";

    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_DRIVE_SPECIAL_SUPER | EntityStateType.CHR_ATK_5P | EntityStateType.CHR_ATK_2P | EntityStateType.CHR_ATK_NORMAL_S | EntityStateType.CHR_ATK_NORMAL_H;
    protected override InputFrame[] requiredInput => new InputFrame[] {new(InputType.P, InputFrameType.PRESSED)};
    public override bool isSelfCancellable => true;

    public override int GetFreezeFrames(Entity to) {
        return 4;
    }
    public override float GetComboProration(Entity to) {
        return .8f;
    }
    public override float GetFirstHitProration(Entity to) {
        return 1f;
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return airborne ? new Vector2(2.5f, .5f) : new Vector2(3f, 0);
    }
    public override Vector2 GetCarriedMomentumPercentage(Entity to) {
        return new(.5f, .2f);
    }
    public override int GetAttackLevel(Entity to) {
        return 1;
    }
}
}
