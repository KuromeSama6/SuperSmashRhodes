using System;
using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_NmlAtk8P : State_Common_AirNormalAttack {
    public State_Common_NmlAtk8P(Entity entity) : base(entity) { } 
    public override EntityStateType type => EntityStateType.CHR_ATK_8P;
    public override float inputPriority => 3;
    protected override string mainAnimation => "cmn/NmlAtk8P";

    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_AIR_NORMAL | EntityStateType.CHR_ATK_DRIVE_SPECIAL_SUPER;
    protected override InputFrame[] requiredInput => new InputFrame[] {new(InputType.P, InputFrameType.PRESSED)};
    public override bool isSelfCancellable => true;

    protected override void OnStateBegin() {
        base.OnStateBegin();
    }

    public override int GetFreezeFrames(Entity to) {
        return 4;
    }
    public override float GetComboProration(Entity to) {
        return .8f;
    }
    public override float GetFirstHitProration(Entity to) {
        return 1f;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(0.5f, airborne ? 5f : 0f);
    }
    public override Vector2 GetCarriedMomentumPercentage(Entity to) {
        return new(.5f, 0f);
    }
    public override int GetAttackLevel(Entity to) {
        return 1;
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.SMALL;
    }
}
}
