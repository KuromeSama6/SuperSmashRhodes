using System;
using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_NmlAtk8S : State_Common_AirNormalAttack {
    public State_Common_NmlAtk8S(Entity entity) : base(entity) { } 
    public override EntityStateType type => EntityStateType.CHR_ATK_8S;
    public override float inputPriority => 3;
    protected override string mainAnimation => "cmn/NmlAtk8S";

    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_NORMAL | EntityStateType.CHR_ATK_DRIVE_SPECIAL_SUPER;
    protected override InputFrame[] requiredInput => new InputFrame[] {new(InputType.S, InputFrameType.PRESSED)};

    public override int GetFreezeFrames(Entity to) {
        return 6;
    }
    public override float GetComboProration(Entity to) {
        return .9f;
    }
    public override float GetFirstHitProration(Entity to) {
        return .9f;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(1.5f, airborne ? 6f : 0f);
    }
    public override Vector2 GetCarriedMomentumPercentage(Entity to) {
        return new(.5f, 0f);
    }
    public override int GetAttackLevel(Entity to) {
        return 2;
    }
    
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.MEDIUM;
    }
}
}
