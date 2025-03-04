using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Animation;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Chen_OvrSklChixiao")]
public class State_Chen_OvrSklChixiao : State_Common_OverdriveAttack {
    public State_Chen_OvrSklChixiao(Entity entity) : base(entity) { }
    protected override string mainAnimation => "chr/OvrSklChixiao";
    public override AttackFrameData frameData => new() {
        startup = 9, active = 5, recovery = 42
    };
    protected override InputFrame[] requiredInput => new InputFrame[] {
        new(InputType.FORWARD, InputFrameType.HELD),
        new(InputType.DOWN, InputFrameType.PRESSED),
        new(InputType.BACKWARD, InputFrameType.PRESSED),
        new(InputType.FORWARD, InputFrameType.PRESSED),
        new(InputType.HS, InputFrameType.PRESSED),
    };

    protected override int framesBeforeSuperfreeze => 7;
    protected override int farHitSkipFrame => 160;
    protected override int farHitActiveFrames => 5;
    protected override float cinematicHitDistance => 3f;
    protected override float cinematicTotalLength => 198;
    public override AttackType invincibility => frame <= 15 ? AttackType.FULL : AttackType.NONE;
    public override StateIndicatorFlag stateIndicator => (invincibility == AttackType.FULL ? StateIndicatorFlag.INVINCIBLE : StateIndicatorFlag.NONE) | base.stateIndicator;
    protected override EntityGhostFXData ghostFXData => new("cb0000".HexToColor(), 0.01333334f, 40, 10f);
    protected override Color cinematicBackgroundColor => "491414".HexToColor();

    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.audioManager.PlaySound("chr/chen/battle/vo/chixiao_start");
        stateData.ghostFXData = new("cb0000".HexToColor(), 0.01333334f, 40, 10f);
    }

    protected override void OnActive() {
        base.OnActive();
        player.audioManager.PlaySound("chr/chen/battle/sfx/drive/p1");
        entity.audioManager.PlaySound($"chr/chen/battle/vo/modal/0");
    }

    protected override void OnSuperHit(bool blocked, bool cinematic) {
        base.OnSuperHit(blocked, cinematic);

        if (cinematic) {
            opponent.stateFlags |= CharacterStateFlag.CAMERA_FOLLOWS_BONE;
            player.stateFlags |= CharacterStateFlag.NO_CAMERA_WEIGHT;
            stateData.cameraData.cameraFovModifier = 15f;
            
            SimpleCameraShakePlayer.inst.Play("chr/chen/battle/fx/camerashake", "chixiao_firsthit");
            opponent.fxManager.PlayGameObjectFX("chr/chen/battle/fx/prefab/skl_214s/hit/0", CharacterFXSocketType.SELF);
            entity.audioManager.PlaySound("chr/chen/battle/sfx/skl_214h/0", .4f);
            // stateData.cameraFocusBone = "632146h";

            player.CallLaterCoroutine(0.3f, () => {
                stateData.cameraData.cameraFovModifier = 5f;
            });
            
            player.CallLaterCoroutine(.8f, () => {
                player.audioManager.PlaySound("chr/chen/battle/vo/632146h");
            });

            player.neutralAniTransitionOverride = .1f;
        }
    }

    public override void OnApplyCinematicDamage(AnimationEventData data) {
        base.OnApplyCinematicDamage(data);
        if (!cinematicHit) return;
        player.audioManager.PlaySound("chr/chen/battle/sfx/chixiao/1", 1.5f, Random.Range(0.8f, 1.2f));
        player.audioManager.PlaySound("chr/chen/battle/sfx/chixiao/1_imp", .4f, Random.Range(0.8f, 1.2f));
        // BackgroundUIManager.inst.Flash(0.03f);
    }

    public override float GetUnscaledDamage(Entity to) {
        return 60f;
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.MEDIUM;
    }
    
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(cinematicHit ? 0 : 1f, 10);
    }

    protected override void OnRelease() {
        base.OnRelease();
        if (cinematicHit) {
            player.opponent.frameData.AddGroundBounce(new(0, 5), BounceFlags.HEAVY);   
            player.opponent.ApplyForwardVelocity(new(0, 5));
            stateData.cameraData = new();
        }
    }

    [AnimationEventHandler("ChiXiao_FinalHit")]
    public virtual void OnFinalHit(AnimationEventData data) {
        player.audioManager.PlaySound("chr/chen/battle/sfx/chixiao/2");

        if (hits > 0) {
            player.opponent.fxManager.PlayGameObjectFX("chr/chen/battle/fx/prefab/skl_632146h/final", CharacterFXSocketType.SELF);
            BackgroundUIManager.inst.Flash(0.1f);
            player.opponent.rotationContainer.localEulerAngles = new(0, 0, Random.Range(-60, 60));

            if (cinematicHit) {
                
                player.opponent.stateFlags &= ~CharacterStateFlag.CAMERA_FOLLOWS_BONE;
                player.stateFlags &= ~CharacterStateFlag.NO_CAMERA_WEIGHT;

                player.opponent.stateFlags |= CharacterStateFlag.NO_CAMERA_WEIGHT;
                player.stateFlags |= CharacterStateFlag.CAMERA_FOLLOWS_BONE;
                stateData.cameraData.focusBone = "632146h";
                stateData.cameraData.cameraWeightModifier = 2f;
                stateData.cameraData.cameraFovModifier = -5f;
                
                ReleaseSocket();
            }
        }
        
        SimpleCameraShakePlayer.inst.Play("chr/chen/battle/fx/camerashake", "chixiao_finalhit");
    }

    [AnimationEventHandler("ChiXiao_Slash")]
    public virtual void OnChiXiaoSlash(AnimationEventData data) {
        if (!cinematicHit) return;
        
        if (data.floatValue == 0) {
            opponent.fxManager.PlayGameObjectFX("chr/chen/battle/fx/prefab/skl_632146h/hit", CharacterFXSocketType.SELF);
        } else {
            opponent.fxManager.PlayGameObjectFX("chr/chen/battle/fx/prefab/skl_632146h/hit_slanted", CharacterFXSocketType.SELF);   
        }
        SimpleCameraShakePlayer.inst.Play("chr/chen/battle/fx/camerashake", "chixiao_hit");
    }
}
}
