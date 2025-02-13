using System.Collections;
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

    public override IEnumerator MainRoutine() {
        player.animation.AddUnmanagedAnimation("std/airdash", false);
        player.rb.linearVelocity = Vector2.zero;
        player.rb.AddForce(player.TranslateDirectionalForce(new(player.characterConfig.airdashSpeedFinal, 0)), ForceMode2D.Impulse);

        var pos = player.transform.position + new Vector3(0, 1, 0);
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/airdash", CharacterFXSocketType.WORLD_UNBOUND, pos);

        player.audioManager.PlaySoundClip("cmn/battle/sfx/movemnt/airdash");
        
        yield return player.characterConfig.airDashDurationFinal;
        
        player.rb.linearVelocityX *= 0.5f;
        CancelInto("CmnAirNeutral");
        stateData.disableSideSwap = true;
        // yield return player.characterConfig.airDashDurationFinal;          
    }

    protected override void OnTick() {
        base.OnTick();
        // add cancel options
        if (frame == player.characterConfig.airDashCancellableFrameFinal) {
            
        }
    }

    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.TimeSlice(player.characterConfig.airDashAvailableFrameFinal + 1).ScanForInput(player.side, new InputFrame(InputType.DASH, InputFrameType.PRESSED));
    }
}
}
