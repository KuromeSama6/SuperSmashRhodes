using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnBurst", 1)]
public class State_CmnBurstParry : State_Common_Parry {
    public State_CmnBurstParry(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_SYSTEMSPECIAL;
    public override float inputPriority => 10;
    protected override string mainAnimation => null;
    public override AttackFrameData frameData => new(0, 16, 39);
    protected override EntityStateType commonCancelOptions => EntityStateType.NONE;
    protected override int normalInputBufferLength => 3;
    protected override float inputMeter => 0;
    public override AttackType invincibility => base.invincibility | (phase == AttackPhase.STARTUP ? AttackType.FULL : AttackType.PROJECTILE);
    public override bool mayEnterState => player.burst.canBurst || true;
    public override StateIndicatorFlag stateIndicator => base.stateIndicator | (phase == AttackPhase.RECOVERY ? StateIndicatorFlag.NONE : StateIndicatorFlag.INVINCIBLE);
    public override bool alwaysCancellable => true;

    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.rb.linearVelocity = Vector2.zero;
        // player.rb.AddForce(new Vector2(0, 15), ForceMode2D.Impulse);
        
        player.burst.AddDeltaTotal(-450, 60);
        player.burst.burstAvailable = false;
        player.burst.burstUsed = true;
        
        var pos = player.transform.position;
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/burstparry/flash", CharacterFXSocketType.WORLD_UNBOUND, pos);
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/burstparry/aura", CharacterFXSocketType.WORLD_UNBOUND, pos);
        entity.PlaySound("cmn/battle/sfx/burst");

        stateData.gravityScale = 0f;
    }

    protected override void OnActive() {
        base.OnActive();
        RemoveCancelOption("CmnWhiteForceReset");
        RemoveCancelOption("CmnDriveRelease");
        player.rb.linearVelocity = Vector2.zero;
    }

    protected override void OnParry(AttackData attack) {
        stateData.backgroundUIData = new(1, .99f, 16f, BackgroundType.NONE, Color.white, 0);
        player.burst.AddDeltaTotal(150, 150);
        entity.PlaySound("cmn/battle/sfx/burstparry/success");
        entity.PlaySound("cmn/battle/sfx/generic/generic_parry_success");
        var pos = player.transform.position;
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/burst", CharacterFXSocketType.WORLD_UNBOUND, pos);
        
        // add time stop frames
        stateData.cameraData.cameraWeightModifier = 0.5f;
        stateData.cameraData.cameraFovModifier = -10f;
        TimeManager.inst.Schedule(0, 60);
        
        SimpleCameraShakePlayer.inst.PlayCommon("burst_parry");
    }

    protected override void OnRecovery() {
        base.OnRecovery();
        stateData.gravityScale = 1;
    }

    public override bool IsInputValid(InputBuffer buffer) {
        var thisFrame = buffer.thisFrame;
        return buffer.TimeSlice(Mathf.Max(1, TimeManager.inst.globalFreezeFrames)).HasInputUnordered(player.side, 
            new(InputType.S, InputFrameType.PRESSED),
            new(InputType.HS, InputFrameType.PRESSED),
            new(InputType.D, InputFrameType.PRESSED),
            new(InputType.P, InputFrameType.PRESSED)
        );
    }
}
}
