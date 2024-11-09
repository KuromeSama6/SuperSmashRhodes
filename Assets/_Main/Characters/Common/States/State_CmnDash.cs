using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {

[NamedToken("CmnDash")]
public class State_CmnDash : EntityState {
    public State_CmnDash(Entity owner) : base(owner, "CmnDash") { }

    public override EntityStateType type => EntityStateType.CHR_MOVEMENT_LOOP;
    public override int inputPriority => 1;

    public override bool mayEnterState {
        get {
            if (!owner.config.mayDash) return false;
            return true;
        }
    }

    public override bool IsInputValid(InputBuffer buffer) {
        if (buffer.thisFrame.HasInput(InputType.BACKWARD, InputFrameType.HELD)) return false;
        
        return buffer.thisFrame.HasInput(InputType.DASH, InputFrameType.HELD);
    }
    
    public override IEnumerator MainRoutine() {
        owner.animation.AddUnmanagedAnimation("std_dash_start", false, .2f);
        owner.animation.AddUnmanagedAnimation("std_dash_loop", true);
        
        while (RevalidateInput()) {
            var force = owner.config.dashAccelCurve.Evaluate(frame);
            owner.rb.AddForceX(PhysicsUtil.NormalizeRelativeDirecionalForce(force, owner.facing));
            owner.rb.linearVelocityX = Mathf.Clamp(owner.rb.linearVelocityX, -owner.config.dashSpeed, owner.config.dashSpeed);
            
            SetScheduledAnimationFrames(1);
            yield return 1;
        }
        
        // state end
        var buffer = GetCurrentInputBuffer();
        if (buffer.thisFrame.HasInput(InputType.FORWARD, InputFrameType.HELD)) {
            CancelInto("CmnMoveForward");
            yield break;
        }
        
        CancelInto("CmnNeutral");
    }
}
}
