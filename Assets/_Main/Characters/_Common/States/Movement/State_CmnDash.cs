using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {

[NamedToken("CmnDash")]
public class State_CmnDash : CharacterState {
    public State_CmnDash(Entity entity) : base(entity) { }

    public override EntityStateType type => EntityStateType.CHR_MOVEMENT_LOOP;
    public override float inputPriority => 1.1f;

    public override bool mayEnterState {
        get {
            if (!player.characterConfig.mayDash) return false;
            return true;
        }
    }

    public override bool IsInputValid(InputBuffer buffer) {
        if (buffer.thisFrame.HasInput(entity.side, InputType.BACKWARD, InputFrameType.HELD)) return false;
        if (buffer.thisFrame.HasInput(entity.side, InputType.DASH, InputFrameType.HELD)) return true;
        var ret = buffer.TimeSlice(10).ScanForInput(entity.side, InputType.FORWARD, InputFrameType.PRESSED, 2);
        return ret;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        AddCancelOption(EntityStateType.CHR_ATK_ALL);
        AddCancelOption(EntityStateType.CHR_MOVEMENT_SINGLE);
        AddCancelOption("CmnNeutralCrouch");
        AddCancelOption("CmnBackdash");
        
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/dash_dust", CharacterFXSocketType.WORLD_UNBOUND, player.transform.position);
    }

    public override IEnumerator MainRoutine() {
        // owner.animation.AddUnmanagedAnimation("std/dash_start", false, .2f);
        entity.animation.AddUnmanagedAnimation("std/dash_loop", true);
        
        while (GetCurrentInputBuffer().thisFrame.HasInput(entity.side, InputType.DASH, InputFrameType.HELD) || GetCurrentInputBuffer().thisFrame.HasInput(entity.side, InputType.FORWARD, InputFrameType.HELD)) {
            var force = player.characterConfig.dashAccelCurve.Evaluate(frame);
            entity.rb.AddForceX(PhysicsUtil.NormalizeSide(force, entity.side));
            entity.rb.linearVelocityX = Mathf.Clamp(entity.rb.linearVelocityX, -player.characterConfig.dashSpeedFinal, player.characterConfig.dashSpeedFinal);
            
            player.meter.AddMeter(0.05f);
            player.meter.balance.value += 0.0012f * player.meter.meterGainMultiplier;
            player.burst.AddDelta(0.03f, 1);
            yield return 1;
        }
        
        // state end
        var buffer = GetCurrentInputBuffer();
        if (buffer.thisFrame.HasInput(entity.side, InputType.FORWARD, InputFrameType.HELD)) {
            CancelInto("CmnMoveForward");
            yield break;
        }
        
    }

    protected override void OnTick() {
        base.OnTick();
    }
}
}
