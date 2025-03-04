using System.Collections;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnJump")]
public class State_CmnJump : CharacterState {
    public State_CmnJump(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_MOVEMENT_SINGLE;
    public override float inputPriority => 2;
    
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.TimeSlice(5).ScanForInput(entity.side, new InputFrame(InputType.UP, InputFrameType.PRESSED));
    }

    public override bool mayEnterState {
        get {
            if (player.airOptions <= 0) return false;
            return true;
        }
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        if (player.airborne) --player.airOptions;
    }

    public override EntityStateSubroutine BeginMainSubroutine() {
        return Sub_PreJump;
    }

    protected virtual void Sub_PreJump(SubroutineContext ctx) {
        entity.animation.AddUnmanagedAnimation("std/prejump", false, .1f);
        player.ApplyGroundedFrictionImmediate();
        
        var prejumpFrames = player.characterConfig.prejumpFinal;
        ctx.Next(prejumpFrames, Sub_JumpMain);
    }

    protected virtual void Sub_JumpMain(SubroutineContext ctx) {
        player.airborne = true;
        entity.animation.AddUnmanagedAnimation("std/jump_up", true);

        float xForce = 0;
        float amount = 3f;

        if (player.inputProvider.inputBuffer.thisFrame.HasInput(entity.side, InputType.DASH, InputFrameType.HELD)) {
            xForce = PhysicsUtil.NormalizeSide(amount * 1.5f, entity.side);
        } if (player.inputProvider.inputBuffer.thisFrame.HasInput(entity.side, InputType.FORWARD, InputFrameType.HELD)) {
            xForce = PhysicsUtil.NormalizeSide(amount, entity.side);
        } else if (player.inputProvider.inputBuffer.thisFrame.HasInput(entity.side, InputType.BACKWARD, InputFrameType.HELD)) {
            xForce = PhysicsUtil.NormalizeSide(-amount, entity.side);
        }
        
        entity.rb.linearVelocity = Vector2.zero;
        stateData.physicsPushboxDisabled = true;
        
        entity.rb.AddForce(new(xForce, player.characterConfig.jumpVelocityFinal), ForceMode2D.Impulse);
        AddCancelOption(EntityStateType.CHR_ATK_AIR_NORMAL | EntityStateType.CHR_ATK_SPECIAL_SUPER);

        ctx.Next(player.characterConfig.airDashAvailableFrameFinal, "CmnAirNeutral");
    }

}
}
