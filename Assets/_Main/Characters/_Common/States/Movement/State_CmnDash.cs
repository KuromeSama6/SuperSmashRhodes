using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {

[NamedToken("CmnDash")]
public class State_CmnDash : CharacterState {
    public State_CmnDash(Entity owner) : base(owner) { }

    public override EntityStateType type => EntityStateType.CHR_MOVEMENT_LOOP;
    public override float inputPriority => 1;

    public override bool mayEnterState {
        get {
            if (!player.characterConfig.mayDash) return false;
            return true;
        }
    }

    public override bool IsInputValid(InputBuffer buffer) {
        if (buffer.thisFrame.HasInput(owner.side, InputType.BACKWARD, InputFrameType.HELD)) return false;
        
        return buffer.thisFrame.HasInput(owner.side, InputType.DASH, InputFrameType.HELD);
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        AddCancelOption(EntityStateType.CHR_ATK_ALL);
        AddCancelOption(EntityStateType.CHR_MOVEMENT_SINGLE);
        AddCancelOption("CmnNeutralCrouch");
        AddCancelOption("CmnBackdash");
    }

    public override IEnumerator MainRoutine() {
        owner.animation.AddUnmanagedAnimation("std/dash_start", false, .2f);
        owner.animation.AddUnmanagedAnimation("std/dash_loop", true);
        
        while (RevalidateInput()) {
            var force = player.characterConfig.dashAccelCurve.Evaluate(frame);
            owner.rb.AddForceX(PhysicsUtil.NormalizeSide(force, owner.side));
            owner.rb.linearVelocityX = Mathf.Clamp(owner.rb.linearVelocityX, -player.characterConfig.dashSpeed, player.characterConfig.dashSpeed);
            
            player.meter.gauge.value += 0.05f * player.meter.meterGainMultiplier;
            player.meter.balance.value += 0.0012f * player.meter.meterGainMultiplier;
            player.burst.AddDelta(0.03f, 1);
            yield return 1;
        }
        
        // state end
        var buffer = GetCurrentInputBuffer();
        if (buffer.thisFrame.HasInput(owner.side, InputType.FORWARD, InputFrameType.HELD)) {
            CancelInto("CmnMoveForward");
            yield break;
        }
        
    }
}
}
