using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnJump")]
public class State_CmnJump : CharacterState {
    public State_CmnJump(Entity owner) : base(owner) { }
    public override EntityStateType type => EntityStateType.CHR_MOVEMENT_SINGLE;
    public override float inputPriority => 2;
    
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.thisFrame.HasInput(InputType.UP, InputFrameType.HELD) 
            || buffer.TimeSlice(5).ScanForInput(new InputFrame(InputType.UP, InputFrameType.PRESSED));
    }
    
    public override IEnumerator MainRoutine() {
        owner.animation.AddUnmanagedAnimation("std_prejump", false, .1f);
        
        var prejumpFrames = owner.config.prejump;
        player.ApplyGroundedFriction(2);
        yield return prejumpFrames;

        player.airborne = true;
        owner.animation.AddUnmanagedAnimation("std_jump_up", true);

        float xForce = 0;
        float amount = 2;
        
        if (player.inputModule.localBuffer.thisFrame.HasInput(InputType.FORWARD, InputFrameType.HELD))
            xForce = PhysicsUtil.NormalizeRelativeDirecionalForce(amount, owner.side);
        else if (player.inputModule.localBuffer.thisFrame.HasInput(InputType.BACKWARD, InputFrameType.HELD))
            xForce = PhysicsUtil.NormalizeRelativeDirecionalForce(-amount, owner.side);
        
        owner.rb.AddForce(new(xForce, owner.config.jumpVelocity), ForceMode2D.Impulse);

        //TODO: Air options available on frame #
        yield return 5;
        
        CancelInto("CmnAirNeutral");
    }
}
}
