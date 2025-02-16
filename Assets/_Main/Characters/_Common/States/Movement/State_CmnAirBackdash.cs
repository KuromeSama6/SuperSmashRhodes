using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnAirBackdash")]
public class State_CmnAirBackdash : CharacterState {
    public State_CmnAirBackdash(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_MOVEMENT_SINGLE;
    public override float inputPriority => 1.5f;
    public override bool IsInputValid(InputBuffer buffer) {
        if (buffer.thisFrame.HasInput(entity.side, InputType.BACKWARD, InputFrameType.HELD) && buffer.TimeSlice(3).ScanForInput(entity.side, new InputFrame(InputType.DASH, InputFrameType.PRESSED))) return true;
        var ret = buffer.TimeSlice(10).ScanForInput(entity.side, InputType.BACKWARD, InputFrameType.PRESSED, 2);
        return ret;
    }
    public override AttackType invincibility => invincible ? AttackType.STRIKE | AttackType.THROW : AttackType.NONE;
    private bool invincible;
    
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
        invincible = true;
        player.airborne = true;
        player.rb.linearVelocity = Vector2.zero;
        stateData.gravityScale = 0;

        var velocity = player.characterConfig.backdashVelocityFinal;
        if (player.atWall) {
            player.rb.AddForceY(velocity.y, ForceMode2D.Impulse);
        } else {
            player.rb.AddForce(player.TranslateDirectionalForce(velocity), ForceMode2D.Impulse);
        }
        
        player.animation.AddUnmanagedAnimation("std/backdash", false);

        // player.neutralAniTransitionOverride = 0f;
        // player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/dash_dust", CharacterFXSocketType.WORLD_UNBOUND, player.transform.position, Vector3.zero, new(-1, 1, 1));
    }
    
    public override IEnumerator MainRoutine() {
        stateData.gravityScale = 0;
        player.animation.AddUnmanagedAnimation("std/backdash", false);
        player.rb.linearVelocity = Vector2.zero;
        player.rb.AddForce(player.TranslateDirectionalForce(new(-player.characterConfig.airBackdashSpeedFinal, 0)), ForceMode2D.Impulse);

        var pos = player.transform.position + new Vector3(0, 1, 0);
        player.audioManager.PlaySoundClip("cmn/battle/sfx/movement/airdash");
        
        yield return player.characterConfig.airBackdashDurationFinal;
        
        player.rb.linearVelocityX *= 0.5f;
        player.neutralAniTransitionOverride = 0.1f;
        CancelInto("CmnAirNeutral");
        stateData.disableSideSwap = true;
        stateData.gravityScale = 1;
    }

    protected override void OnTick() {
        base.OnTick();
        if (frame == player.characterConfig.airDashCancellableFrameFinal) {
            AddCancelOption(EntityStateType.CHR_ATK_ALL);
            AddCancelOption(EntityStateType.CHR_ATK_THROW);
        }
    }
}
}
