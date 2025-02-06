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
        return buffer.thisFrame.HasInput(owner.side, InputType.UP, InputFrameType.HELD) 
            || buffer.TimeSlice(5).ScanForInput(owner.side, new InputFrame(InputType.UP, InputFrameType.PRESSED));
    }
    
    public override IEnumerator MainRoutine() {
        owner.animation.AddUnmanagedAnimation("std/prejump", false, .1f);
        
        var prejumpFrames = player.characterConfig.prejump;
        var originalX = player.rb.linearVelocityX;
        
        player.ApplyGroundedFrictionImmediate();
        yield return prejumpFrames;

        player.airborne = true;
        owner.animation.AddUnmanagedAnimation("std/jump_up", true);

        float xForce = 0;
        float amount = 3f;

        if (player.inputProvider.inputBuffer.thisFrame.HasInput(owner.side, InputType.DASH, InputFrameType.HELD)) {
            xForce = PhysicsUtil.NormalizeSide(amount * 1.5f, owner.side);
        } if (player.inputProvider.inputBuffer.thisFrame.HasInput(owner.side, InputType.FORWARD, InputFrameType.HELD)) {
            xForce = PhysicsUtil.NormalizeSide(amount, owner.side);
        } else if (player.inputProvider.inputBuffer.thisFrame.HasInput(owner.side, InputType.BACKWARD, InputFrameType.HELD)) {
            xForce = PhysicsUtil.NormalizeSide(-amount, owner.side);
        }
        
        owner.rb.AddForce(new(xForce, player.characterConfig.jumpVelocity), ForceMode2D.Impulse);

        //TODO: Air options available on frame #
        yield return 5;
        
        CancelInto("CmnAirNeutral");
    }
}
}
