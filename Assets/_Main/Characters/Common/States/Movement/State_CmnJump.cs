using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnJump")]
public class State_CmnJump : CharacterState {
    public State_CmnJump(Entity owner) : base(owner) { }
    public override EntityStateType type => EntityStateType.CHR_MOVEMENT_SINGLE;
    public override int inputPriority => 2;
    
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.thisFrame.HasInput(InputType.UP, InputFrameType.HELD) 
            || buffer.TimeSlice(5).ScanForInput(new InputFrame(InputType.UP, InputFrameType.PRESSED));
    }
    
    public override IEnumerator MainRoutine() {
        owner.animation.AddUnmanagedAnimation("std_prejump", false, .1f);
        
        var prejumpFrames = owner.config.prejump;
        yield return prejumpFrames;

        player.airborne = true;
        owner.animation.AddUnmanagedAnimation("std_jump_up", true);
        owner.rb.AddForceY(owner.config.jumpVelocity, ForceMode2D.Impulse); 

        //TODO: Air options available on frame #
        yield return 5;
        
        CancelInto("CmnAirNeutral");
    }
}
}
