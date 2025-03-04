using System.Collections;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Util;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnMoveForward")]
public class State_CmnMoveForward : CharacterState {
    public State_CmnMoveForward(Entity entity) : base(entity) { }

    public override EntityStateType type => EntityStateType.CHR_MOVEMENT_LOOP;
    public override float inputPriority => 1;
    protected override SubroutineFlags mainRoutineFlags => SubroutineFlags.NO_PRETICK_SUBROUTINES;
    
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.thisFrame.HasInput(entity.side, InputType.FORWARD, InputFrameType.HELD) &&
               !buffer.thisFrame.HasInput(entity.side, InputType.BACKWARD, InputFrameType.HELD);
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        AddCancelOption("CmnDash");
        AddCancelOption("CmnJump");
        AddCancelOption(EntityStateType.CHR_ATK_ALL);
        AddCancelOption("CmnNeutralCrouch");
        AddCancelOption("CmnBackdash");
        
        stateData.maySwitchSides = true;
    }

    public override EntityStateSubroutine BeginMainSubroutine() {
        entity.animation.AddUnmanagedAnimation("std/walk", true, .05f);
        return Sub_MoveLoop;
    }

    protected virtual void Sub_MoveLoop(SubroutineContext ctx) {
        if (RevalidateInput()) {
            entity.rb.AddForceX(PhysicsUtil.NormalizeSide(50, entity.side));
            entity.rb.linearVelocityX = Mathf.Clamp(entity.rb.linearVelocityX, -player.characterConfig.walkSpeedFinal, player.characterConfig.walkSpeedFinal);
            // 0.01% meter gain per frame
            player.meter.AddMeter(0.02f);
            player.meter.balance.value += 0.0007f;
            player.burst.AddDelta(0.015f, 1);
            ctx.Repeat();
            return;
            
        } else {
            ctx.Exit();   
        }
    }
}

[NamedToken("CmnMoveBackward")]
public class State_CmnMoveBackward : CharacterState {
    public State_CmnMoveBackward(Entity entity) : base(entity) { }

    public override EntityStateType type => EntityStateType.CHR_MOVEMENT_LOOP;
    public override float inputPriority => 1;
    protected override SubroutineFlags mainRoutineFlags => SubroutineFlags.NO_PRETICK_SUBROUTINES;
    
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.thisFrame.HasInput(entity.side, InputType.BACKWARD, InputFrameType.HELD)
            && !buffer.thisFrame.HasInput(entity.side, InputType.FORWARD, InputFrameType.HELD);
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        
        AddCancelOption("CmnDash");
        AddCancelOption("CmnJump");
        AddCancelOption(EntityStateType.CHR_ATK_ALL);
        AddCancelOption("CmnNeutralCrouch");
        AddCancelOption("CmnBackdash");
        
        stateData.maySwitchSides = true;
    }

    public override EntityStateSubroutine BeginMainSubroutine() {
        entity.animation.AddUnmanagedAnimation("std/walk", true, .05f);
        return Sub_MoveLoop;
    }

    protected virtual void Sub_MoveLoop(SubroutineContext ctx) {
        if (RevalidateInput()) {
            entity.rb.AddForceX(PhysicsUtil.NormalizeSide(-50, entity.side));
            entity.rb.linearVelocityX = Mathf.Clamp(entity.rb.linearVelocityX, -player.characterConfig.backwalkSpeedFinal, player.characterConfig.backwalkSpeedFinal);
            
            player.burst.AddDelta(-0.1f, 1);
            ctx.Repeat();
            
        } else {
            ctx.Exit();
        }
    }
    
}


}
