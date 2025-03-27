using System;
using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_NmlAtk2S : State_Common_NormalAttack {
    public State_Common_NmlAtk2S(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_2S;
    public override float inputPriority => 4;

    protected override string mainAnimation => "cmn/NmlAtk2S";

    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_NORMAL_S | EntityStateType.CHR_ATK_NORMAL_H | EntityStateType.CHR_ATK_DRIVE_SPECIAL_SUPER;
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.thisFrame.HasInput(player.side, InputType.DOWN, InputFrameType.HELD) && 
               buffer.TimeSlice(normalInputBufferLength).ScanForInput(player.side, new InputFrame(InputType.S, InputFrameType.PRESSED));
    }
    public override float GetComboProration(Entity to) {
        return .9f;
    }
    public override float GetFirstHitProration(Entity to) {
        return .9f;
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.CROUCHING;
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
