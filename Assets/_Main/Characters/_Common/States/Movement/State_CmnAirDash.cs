using System.Collections;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnAirDash")]
public class State_CmnAirDash : CharacterState {
    public State_CmnAirDash(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_MOVEMENT_SINGLE;
    public override float inputPriority => 1;
    public override bool mayEnterState {
        get {
            if (!player.airborne) return false;
            if (!player.characterConfig.mayAirDash) return false;
            if (player.airOptions <= 0) return false;
            return true;
        }
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.airOptions--;
    }

    public override EntityStateSubroutine BeginMainSubroutine() {
        return Sub_DashMain;
    }

    protected virtual void Sub_DashMain(SubroutineContext ctx) {
        stateData.gravityScale = 0;
        player.animation.AddUnmanagedAnimation("std/airdash", false);
        player.rb.linearVelocity = Vector2.zero;
        player.rb.AddForce(player.TranslateDirectionalForce(new(player.characterConfig.airDashSpeedFinal, 0)), ForceMode2D.Impulse);

        var pos = player.transform.position + new Vector3(0, 1, 0);
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/airdash", CharacterFXSocketType.WORLD_UNBOUND, pos);

        player.audioManager.PlaySoundClip("cmn/battle/sfx/movement/airdash");
        
        ctx.Next(player.characterConfig.airDashDurationFinal, "CmnAirNeutral");
    }

    protected override void OnStateEndComplete(EntityState nextState) {
        base.OnStateEndComplete(nextState);
        player.rb.linearVelocityX *= 0.5f;
        stateData.disableSideSwap = true;
    }

    protected override void OnTick() {
        base.OnTick();
        // add cancel options
        if (frame == player.characterConfig.airDashCancellableFrameFinal) {
            AddCancelOption(EntityStateType.CHR_ATK_ALL);
            AddCancelOption(EntityStateType.CHR_ATK_THROW);
        }
    }

    public override bool IsInputValid(InputBuffer buffer) {
        if (buffer.TimeSlice(3).ScanForInput(entity.side, new InputFrame(InputType.DASH, InputFrameType.PRESSED))) return true;
        var ret = buffer.TimeSlice(15).ScanForInput(entity.side, InputType.BACKWARD, InputFrameType.PRESSED, 2);
        return ret;
    }
}
}
