using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using SuperSmashRhodes.UI.Battle.Burst;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnDriveRelease")]
public class State_CmnDriveRelease : CharacterState {
    public State_CmnDriveRelease(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_DRIVE_RELEASE;
    public override float inputPriority => 9;
    public override StateIndicatorFlag stateIndicator => isReleaseCancel ? StateIndicatorFlag.DRIVE_RELEASE_CANCEL : StateIndicatorFlag.DRIVE_RELEASE;
    public override bool IsInputValid(InputBuffer buffer) {
        var thisFrame = buffer.TimeSlice(6);
        return thisFrame.HasInputUnordered(player.side, new[] {
            new InputFrame(InputType.P, InputFrameType.PRESSED),
            new InputFrame(InputType.S, InputFrameType.PRESSED),
            new InputFrame(InputType.HS, InputFrameType.PRESSED),
        });
    }

    public override bool mayEnterState => player.burst.canDriveRelease;
    public override CharacterStateFlag globalFlags => CharacterStateFlag.PAUSE_GAUGE | CharacterStateFlag.GLOBAL_PAUSE_TIMER;
    private bool isReleaseCancel;

    protected override void OnStateBegin() {
        base.OnStateBegin();
        isReleaseCancel = false;
        opponent.comboCounter.comboDecay = 0f;
        
        opponent.rb.linearVelocity = new(0, Mathf.Max(0, opponent.rb.linearVelocity.y));
        
        if (opponent.activeState is State_CmnHitStunAir) {
            opponent.rb.AddForceY(3f, ForceMode2D.Impulse);
        }
        
        player.ResetAirOptions();
        player.fxManager.PlayEmblemFX(1f, false);
    }

    protected override void OnStateEnd(string nextState) {
        base.OnStateEnd(nextState);
    }

    public override IEnumerator MainRoutine() {
        player.audioManager.PlaySound("cmn/battle/sfx/driverelease");
        
        var pos = player.transform.position;
        player.rb.linearVelocity = Vector2.zero;
        player.audioManager.PlaySound($"chr/{player.config.id}/battle/vo/driverelease");
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/super/smoke", CharacterFXSocketType.WORLD_UNBOUND, pos);
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/driverelease/star", CharacterFXSocketType.WORLD_UNBOUND, pos);
        
        PortraitCutscenePlayer.Get(player.playerIndex).Play(player.descriptor.portrait, 1f);
        
        float frames;
        
        if (player.lastState != null) {
            if (player.lastState is CharacterAttackStateBase) isReleaseCancel = true;
            if (player.lastState is State_Common_Stun) isReleaseCancel = true;
        }
        
        if (!isReleaseCancel) {
            frames = 4 * 60;
        } else {
            frames = 2.5f * 60;
        }

        if (player.burst.gauge.value >= 600f) frames *= 1.5f;
        if (player.health / player.config.health <= .2f) frames *= 1.5f;
        
        frames = Mathf.Min(frames, player.burst.maxReleaseFrames);
        
        player.burst.BeginDriveRelease((int)frames);
        var counter = DriveReleaseGaugeUI.Get(player.playerIndex).counter;
        counter.target = frames / 60f * 10f;
        counter.ApplyImmediately();
        
        // Superfreeze effects
        stateData.backgroundUIData.priority = 10;
        stateData.backgroundUIData.dimAlpha = 0.995f;
        stateData.backgroundUIData.dimSpeed = 10;
        TimeManager.inst.globalFreezeFrames = 60;
        yield return 1;
    }
}
}
