using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnMoveForward")]
public class State_CmnMoveForward : EntityState {
    public State_CmnMoveForward(Entity owner) : base(owner, "CmnMoveForward") { }

    public override EntityStateType type => EntityStateType.CHR_MOVEMENT_LOOP;
    public override int inputPriority => 1;
    
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.thisFrame.HasInput(InputType.FORWARD, InputFrameType.HELD) &&
               !buffer.thisFrame.HasInput(InputType.BACKWARD, InputFrameType.HELD);
    }
    
    public override IEnumerator MainRoutine() {
        AddCancelOption("CmnDash");
        owner.animation.AddUnmanagedAnimation("std_walk", true, .2f);
        
        while (RevalidateInput()) {
            owner.rb.AddForceX(PhysicsUtil.NormalizeRelativeDirecionalForce(50, owner.facing));
            owner.rb.linearVelocityX = Mathf.Clamp(owner.rb.linearVelocityX, -owner.config.walkSpeed, owner.config.walkSpeed );
            
            SetScheduledAnimationFrames(1);
            yield return 1;
        }
        
        CancelInto("CmnNeutral");
    }
}

[NamedToken("CmnMoveBackward")]
public class State_CmnMoveBackward : EntityState {
    public State_CmnMoveBackward(Entity owner) : base(owner, "CmnMoveBackward") { }

    public override EntityStateType type => EntityStateType.CHR_MOVEMENT_LOOP;
    public override int inputPriority => 1;
    
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.thisFrame.HasInput(InputType.BACKWARD, InputFrameType.HELD)
            && !buffer.thisFrame.HasInput(InputType.FORWARD, InputFrameType.HELD);
    }
    
    public override IEnumerator MainRoutine() {
        owner.animation.AddUnmanagedAnimation("std_walk", true, .2f);
        
        while (RevalidateInput()) {
            owner.rb.AddForceX(PhysicsUtil.NormalizeRelativeDirecionalForce(-50, owner.facing));
            owner.rb.linearVelocityX = Mathf.Clamp(owner.rb.linearVelocityX, -owner.config.backwalkSpeed, owner.config.backwalkSpeed);
            
            SetScheduledAnimationFrames(1);
            yield return 1;
        }
        
        CancelInto("CmnNeutral");
    }
}


}
