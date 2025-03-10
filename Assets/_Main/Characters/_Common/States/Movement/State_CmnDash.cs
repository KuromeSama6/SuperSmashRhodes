using System.Collections;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Enums;
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
    protected override SubroutineFlags mainRoutineFlags => SubroutineFlags.NO_PRETICK_SUBROUTINES;
    
    
    public override bool mayEnterState {
        get {
            if (!player.characterConfig.mayDash) return false;
            return true;
        }
    }

    public override bool IsInputValid(InputBuffer buffer) {
        if (buffer.thisFrame.HasInput(entity.side, InputType.BACKWARD, InputFrameType.HELD)) return false;
        if (buffer.thisFrame.HasInput(entity.side, InputType.DASH, InputFrameType.HELD)) return true;
        var ret = buffer.TimeSlice(15).ScanForInput(entity.side, InputType.FORWARD, InputFrameType.PRESSED, 2);
        return ret;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        AddCancelOption(EntityStateType.CHR_ATK_ALL);
        AddCancelOption(EntityStateType.CHR_MOVEMENT_SINGLE);
        AddCancelOption("CmnNeutralCrouch");
        AddCancelOption("CmnBackdash");
        
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/dash_dust", CharacterFXSocketType.WORLD_UNBOUND, player.transform.position, Vector3.zero, new Vector3(player.side == EntitySide.LEFT ? 1 : -1, 1, 1));
        stateData.disableSideSwap = true;
    }

    public override EntityStateSubroutine BeginMainSubroutine() {
        // owner.animation.AddUnmanagedAnimation("std/dash_start", false, .2f);
        entity.animation.AddUnmanagedAnimation("std/dash_loop", true);
        return Sub_DashLoop;
    }

    protected virtual void Sub_DashLoop(SubroutineContext ctx) {
        if (GetCurrentInputBuffer().thisFrame.HasInput(entity.side, InputType.DASH, InputFrameType.HELD) || GetCurrentInputBuffer().thisFrame.HasInput(entity.side, InputType.FORWARD, InputFrameType.HELD)) {
            var force = player.characterConfig.dashAccelCurve.Evaluate(frame);
            entity.rb.AddForceX(PhysicsUtil.NormalizeSide(force, entity.side));
            entity.rb.linearVelocityX = Mathf.Clamp(entity.rb.linearVelocityX, -player.characterConfig.dashSpeedFinal, player.characterConfig.dashSpeedFinal);
            
            player.meter.AddMeter(0.05f);
            player.meter.balance.value += 0.0012f * player.meter.meterGainMultiplier;
            player.burst.AddDelta(0.03f, 1);
            ctx.Repeat();
            
        } else {
            var buffer = GetCurrentInputBuffer();
            
            if (buffer.thisFrame.HasInput(entity.side, InputType.FORWARD, InputFrameType.HELD)) {
                CancelInto("CmnMoveForward");
            } else {
                ctx.Exit();
            }
        }
    }
    
    protected override void OnTick() {
        base.OnTick();
    }
}
}
