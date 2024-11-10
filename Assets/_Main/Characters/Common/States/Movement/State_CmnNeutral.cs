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
    public override int inputPriority => -1;
    public override EntityStateType type => EntityStateType.CHR_NEUTRAL;

    public State_CmnNeutral(Entity owner) : base(owner) { }
    
    public override bool IsInputValid(InputBuffer buffer) {
        return false;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        AddCancelOption(EntityStateType.ALL);
    }

    public override IEnumerator MainRoutine() {
        owner.animation.AddUnmanagedAnimation("std_neutral", true);

        while (true) {
            player.ApplyGroundedFriction();
            yield return 1;
        }
        
    }
}

[NamedToken("CmnAirNeutral")]
public class State_CmnAirNeutral : CharacterState {
    public State_CmnAirNeutral(Entity owner) : base(owner) { }
    public override EntityStateType type => EntityStateType.CHR_NEUTRAL;
    public override int inputPriority => -1;
    public override bool IsInputValid(InputBuffer buffer) {
        return false;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        AddCancelOption(EntityStateType.CHR_ATK_AIR_NORMAL | EntityStateType.CHR_ATK_SPECIAL_SUPER);
    }

    public override IEnumerator MainRoutine() {
        owner.animation.AddUnmanagedAnimation("std_jump_up", true);
        
        while (owner.rb.linearVelocityY > 0f) yield return 1;
        
        owner.animation.AddUnmanagedAnimation("std_jump_down", true, 0.1f);
        while (player.airborne) yield return 1;
        
        owner.animation.AddUnmanagedAnimation("std_land", false);
        // TODO: Landing recovery
        player.ApplyGroundedFriction(7);
        yield return 7;
    }
}

[NamedToken("CmnNeutralCrouch")]
public class State_CmnNeutralCrouch : CharacterState {
    public State_CmnNeutralCrouch(Entity owner) : base(owner) { }
    public override EntityStateType type => EntityStateType.CHR_NEUTRAL;
    public override int inputPriority => 0;
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.thisFrame.HasInput(InputType.DOWN, InputFrameType.HELD);
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        AddCancelOption(EntityStateType.CHR_ATK_ALL);
    }

    public override IEnumerator MainRoutine() {
        owner.animation.AddUnmanagedAnimation("std_crouch", true);
        while (RevalidateInput()) {
            player.ApplyGroundedFriction();
            yield return 1;
        }
    }
}
}
