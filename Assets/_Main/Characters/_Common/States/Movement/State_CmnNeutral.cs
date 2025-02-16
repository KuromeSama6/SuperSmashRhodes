using System.Collections;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnNeutral")]
public class State_CmnNeutral : CharacterState {
    public override float inputPriority => -1;
    public override EntityStateType type => EntityStateType.CHR_NEUTRAL;

    public State_CmnNeutral(Entity entity) : base(entity) { }
    
    public override bool IsInputValid(InputBuffer buffer) {
        return false;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        AddCancelOption(EntityStateType.ALL & ~EntityStateType.CHR_ATK_SPECIAL_TRIGGER);
        player.comboCounter.Reset();
    }

    public override IEnumerator MainRoutine() {
        entity.animation.AddUnmanagedAnimation("std/neutral", true, player.neutralAniTransitionOverride);
        player.neutralAniTransitionOverride = 0.05f;

        while (true) {
            player.ApplyGroundedFriction();
            yield return 1;
        }
        
    }
}


[NamedToken("CmnNeutralCrouch")]
public class State_CmnNeutralCrouch : CharacterState {
    public State_CmnNeutralCrouch(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_NEUTRAL;
    public override float inputPriority => 0;
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.thisFrame.HasInput(entity.side, InputType.DOWN, InputFrameType.HELD);
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        AddCancelOption(EntityStateType.CHR_ATK_ALL);
        AddCancelOption(EntityStateType.CHR_DRIVE_RELEASE);
        player.comboCounter.Reset();
    }

    public override IEnumerator MainRoutine() {
        entity.animation.AddUnmanagedAnimation("std/crouch", true, player.neutralAniTransitionOverride);
        player.neutralAniTransitionOverride = 0.05f;
        while (RevalidateInput()) {
            player.ApplyGroundedFriction();
            yield return 1;
        }
    }
}

[NamedToken("CmnAirNeutral")]
public class State_CmnAirNeutral : CharacterState {
    public State_CmnAirNeutral(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_NEUTRAL;
    public override float inputPriority => -1;

    private bool cancelOptionsAdded;
    
    public override bool IsInputValid(InputBuffer buffer) {
        return false;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        cancelOptionsAdded = false;
        TryAddCancelOptions();
        player.comboCounter.Reset();
        
        AddCancelOption(EntityStateType.CHR_DRIVE_RELEASE);
    }

    public override IEnumerator MainRoutine() {
        entity.animation.AddUnmanagedAnimation("std/jump_up", true, player.neutralAniTransitionOverride);
        
        while (entity.rb.linearVelocityY > 0f) yield return 1;
        
        entity.animation.AddUnmanagedAnimation("std/jump_down", true, 0.1f);
        while (player.airborne) yield return 1;
    }

    public override void OnLand(LandingRecoveryFlag flag, int recoveryFrames) {
        base.OnLand(flag, recoveryFrames);
        CancelInto("CmnLandingRecovery");
    }

    protected override void OnTick() {
        base.OnTick();
        TryAddCancelOptions();
    }

    private void TryAddCancelOptions() {
        if (cancelOptionsAdded) return;
        if (!player.frameData.landingFlag.HasFlag(LandingRecoveryFlag.UNTIL_LAND) || !player.airborne) {
            AddCancelOption(EntityStateType.CHR_ATK_AIR_NORMAL | EntityStateType.CHR_ATK_THROW | EntityStateType.CHR_ATK_SPECIAL_SUPER);
            AddCancelOption("CmnAirDash");
            AddCancelOption("CmnAirBackdash");
            AddCancelOption("CmnJump");
        }
    }
}

}
