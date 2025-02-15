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

    public override IEnumerator MainRoutine() {
        entity.animation.AddUnmanagedAnimation("std/prejump", false, .1f);
        
        var prejumpFrames = player.characterConfig.prejumpFinal;
        var originalX = player.rb.linearVelocityX;
        
        player.ApplyGroundedFrictionImmediate();
        yield return prejumpFrames;

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
        entity.rb.AddForce(new(xForce, player.characterConfig.jumpVelocityFinal), ForceMode2D.Impulse);
        
        AddCancelOption(EntityStateType.CHR_ATK_AIR_NORMAL | EntityStateType.CHR_ATK_SPECIAL_SUPER);

        //TODO: Air options available on frame #
        // Debug.Log(player.characterConfig.airDashAvailableFrameFinal);
        yield return player.characterConfig.airDashAvailableFrameFinal;
        // Debug.Log("ok");
        CancelInto("CmnAirNeutral");
    }
}
}
