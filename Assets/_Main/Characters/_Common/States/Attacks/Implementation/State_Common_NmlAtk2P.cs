﻿using System;
using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_NmlAtk2P : State_Common_NormalAttack {
    public State_Common_NmlAtk2P(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_2P;
    public override float inputPriority => 4;

    protected override string mainAnimation => "cmn/NmlAtk2P";

    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_NORMAL | EntityStateType.CHR_ATK_DRIVE_SPECIAL_SUPER;
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.thisFrame.HasInput(player.side, InputType.DOWN, InputFrameType.HELD) && 
               buffer.TimeSlice(normalInputBufferLength).ScanForInput(player.side, new InputFrame(InputType.P, InputFrameType.PRESSED));
    }

    public override bool isSelfCancellable => true;

    public override void OnContact(Entity to) {
        base.OnContact(to);
        AddCancelOption("CmnJump");
    }
    public override float GetComboProration(Entity to) {
        return .8f;
    }
    public override float GetFirstHitProration(Entity to) {
        return 1f;
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.CROUCHING;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return airborne ? new Vector2(2f, .5f) : new Vector2(3.5f, 0);
    }
    public override Vector2 GetCarriedMomentumPercentage(Entity to) {
        return new(.5f, .2f);
    }
    public override int GetAttackLevel(Entity to) {
        return 1;
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.SMALL;
    }
}
}
