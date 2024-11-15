using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnMoveForward")]
public class State_CmnMoveForward : CharacterState {
    public State_CmnMoveForward(Entity owner) : base(owner) { }

    public override EntityStateType type => EntityStateType.CHR_MOVEMENT_LOOP;
    public override float inputPriority => 1;
    
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.thisFrame.HasInput(InputType.FORWARD, InputFrameType.HELD) &&
               !buffer.thisFrame.HasInput(InputType.BACKWARD, InputFrameType.HELD);
    }

    protected override void OnStateBegin() {
        AddCancelOption("CmnDash");
        AddCancelOption("CmnJump");
        AddCancelOption(EntityStateType.CHR_ATK_ALL);
        AddCancelOption("CmnNeutralCrouch");
    }

    public override IEnumerator MainRoutine() {
        owner.animation.AddUnmanagedAnimation("std_walk", true);
        
        while (RevalidateInput()) {
            owner.rb.AddForceX(PhysicsUtil.NormalizeRelativeDirecionalForce(50, owner.side));
            owner.rb.linearVelocityX = Mathf.Clamp(owner.rb.linearVelocityX, -player.characterConfig.walkSpeed, player.characterConfig.walkSpeed);
            // 0.01% meter gain per frame
            player.meter.meter.value += 0.02f;
            player.meter.meterBalance.value += 0.0007f;
            yield return 1;
        }
    }
}

[NamedToken("CmnMoveBackward")]
public class State_CmnMoveBackward : CharacterState {
    public State_CmnMoveBackward(Entity owner) : base(owner) { }

    public override EntityStateType type => EntityStateType.CHR_MOVEMENT_LOOP;
    public override float inputPriority => 1;
    
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.thisFrame.HasInput(InputType.BACKWARD, InputFrameType.HELD)
            && !buffer.thisFrame.HasInput(InputType.FORWARD, InputFrameType.HELD);
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        
        AddCancelOption("CmnDash");
        AddCancelOption("CmnJump");
        AddCancelOption(EntityStateType.CHR_ATK_ALL);
        AddCancelOption("CmnNeutralCrouch");
    }

    public override IEnumerator MainRoutine() {
        owner.animation.AddUnmanagedAnimation("std_walk", true);
        
        while (RevalidateInput()) {
            owner.rb.AddForceX(PhysicsUtil.NormalizeRelativeDirecionalForce(-50, owner.side));
            owner.rb.linearVelocityX = Mathf.Clamp(owner.rb.linearVelocityX, -player.characterConfig.backwalkSpeed, player.characterConfig.backwalkSpeed);
            
            
            yield return 1;
        }
    }
}


}
