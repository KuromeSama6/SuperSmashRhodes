using System;
using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_NmlAtk6S : State_Common_NormalAttack {
    public State_Common_NmlAtk6S(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_6S;
    public override float inputPriority => 3.2f;

    protected override string mainAnimation => "cmn/NmlAtk6S";

    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_NORMAL | EntityStateType.CHR_ATK_DRIVE_SPECIAL_SUPER;
    protected override InputFrame[] requiredInput => new InputFrame[] {new(InputType.FORWARD, InputFrameType.HELD), new(InputType.S, InputFrameType.PRESSED)};

    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.thisFrame.HasInput(player.side, InputType.FORWARD, InputFrameType.HELD) && 
               buffer.TimeSlice(normalInputBufferLength).ScanForInput(player.side, new InputFrame(InputType.S, InputFrameType.PRESSED));
    }

    public override void OnContact(Entity to) {
        base.OnContact(to);
        AddCancelOption("CmnJump");
    }
    public override int GetFreezeFrames(Entity to) {
        return 6;
    }
    public override float GetComboProration(Entity to) {
        return .9f;
    }
    public override float GetFirstHitProration(Entity to) {
        return .9f;
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.STANDING;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (airborne) return new Vector2(5f, 2.5f);
        return blocked ? new Vector2(5f, 0) : new Vector2(2.5f, 8);
    }
    public override Vector2 GetCarriedMomentumPercentage(Entity to) {
        return new(.5f, .2f);
    }
    public override int GetAttackLevel(Entity to) {
        return 3;
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.MEDIUM;
    }
}
}
