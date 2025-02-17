using System.Collections;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Animation;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_OverdriveAttack : CharacterAttackStateBase {
    public State_Common_OverdriveAttack(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_SUPER;
    public override float inputPriority => 6f;
    protected override EntityStateType commonCancelOptions => EntityStateType.NONE;
    protected override int normalInputBufferLength => 20;
    protected override float inputMeter => 0;
    public override bool mayEnterState => player.meter.gauge.value >= meterCost || true;
    public override CharacterStateFlag globalFlags => CharacterStateFlag.PAUSE_GAUGE | CharacterStateFlag.GLOBAL_PAUSE_TIMER;
    public override StateIndicatorFlag stateIndicator => StateIndicatorFlag.SUPER | (player.burst.driveRelease ? StateIndicatorFlag.DRIVE_RELEASE : StateIndicatorFlag.NONE);

    
    protected CinematicCharacterSocket socket { get; private set; }
    protected bool cinematicHit { get; private set; }
    protected bool hasHit { get; private set; }
    protected bool cinematicHitWindow { get; private set; }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.meter.gauge.value -= meterCost;
        cinematicHit = false;
        hasHit = false;
        cinematicHitWindow = true;
        
        if (socket != null) {
            socket.Release();
            socket = null;   
        }
        
        player.fxManager.PlayEmblemFX(1f, true);
    }
    
    public override IEnumerator MainRoutine() {
        entity.animation.AddUnmanagedAnimation(mainAnimation, false);
        phase = AttackPhase.STARTUP;
        OnStartup();
        player.audioManager.PlaySound("cmn/battle/sfx/super");
        
        var pos = player.transform.position;
        player.rb.linearVelocity = Vector2.zero;
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/super/smoke", CharacterFXSocketType.WORLD_UNBOUND, pos);
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/super/star", CharacterFXSocketType.WORLD_UNBOUND, pos);

        PortraitCutscenePlayer.Get(player.playerIndex).Play(player.descriptor.superPortrait, 1f);
        
        player.ApplyGroundedFriction(frameData.startup);
        // Superfreeze effects
        stateData.backgroundUIData.priority = 20;
        stateData.backgroundUIData.dimAlpha = 0.98f;
        stateData.backgroundUIData.dimSpeed = 10;
        yield return framesBeforeSuperfreeze;
        
        TimeManager.inst.globalFreezeFrames = superfreezeLength;
        yield return 1;
        
        yield return frameData.startup - framesBeforeSuperfreeze;

        phase = AttackPhase.ACTIVE;
        OnActive();
        player.ApplyGroundedFriction(frameData.active);
        entity.audioManager.PlaySound(GetAttackNormalSfx());
        
        // check for cinematic hit
        cinematicHitWindow = true;
        for (int i = 0; i < frameData.active; i++) {
            // Debug.Log($"active, {hitsRemaining} rem");
            if (cinematicHit) {
                // Debug.Log("cinematic hit");
                break;
            }
            yield return 1;
        }
        cinematicHitWindow = false;
        
        if (cinematicHit) {
            // big cinematic effect!!!
            stateData.gravityScale = 0;
            stateData.ghostFXData = new(Color.white);
            socket = new CinematicCharacterSocket(opponent, player, "throw_opponent", new(0, 0, 0));
            
            socket.Attach();
            
            // superfreeze part 2
            stateData.backgroundUIData.dimAlpha = 1f;
            stateData.backgroundUIData.bgType = BackgroundType.SUPER;
            stateData.backgroundUIData.bgColor = "2B395C".HexToColor();
            stateData.backgroundUIData.transitionFrame = 0;
            stateData.backgroundUIData.transition = TransitionType.SUPER_FADE_IN;

            TimeManager.inst.globalFreezeFrames = superfreezeHitstopLength;
            yield return 1;
            // Debug.Log("continue");

            var length = cinematicTotalLength - frameData.active - frameData.startup;
            // Debug.Log(length);
            yield return (int)length;
            // Debug.Log("end");
            
            opponent.stateFlags = player.stateFlags = default;

        } else {
            if (hitsRemaining > 0) OnWhiff();
            FastForward(farHitSkipFrame - frame);
            // Debug.Log("nocin");
            opponent.stateFlags = player.stateFlags = default;
            yield return farHitActiveFrames;
            
            // Debug.Log("1");
            player.ApplyGroundedFriction(frameData.active);
            phase = AttackPhase.RECOVERY;
            OnRecovery();
            yield return frameData.recovery;
            // Debug.Log("recovery");
        }
    }

    protected override void OnStateEnd(string nextState) {
        base.OnStateEnd(nextState);
        if (socket != null && socket.attached) {
            socket.Release();
            opponent.stateFlags = player.stateFlags = default;
        }

        player.burst.releaseFrames = 0;
    }

    protected override void OnTick() {
        base.OnTick();
        if (socket != null && socket.attached) socket.Tick();
    }

    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(10, 5);
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    
    public override float GetChipDamagePercentage(Entity to) {
        return .25f;
    }
    public override float GetOtgDamagePercentage(Entity to) {
        return .8f;
    }
    public override float GetComboProration(Entity to) {
        return 1f;
    }
    public override float GetFirstHitProration(Entity to) {
        return 1f;
    }
    public override int GetFreezeFrames(Entity to) {
        return 0;
    }
    public override int GetAttackLevel(Entity to) {
        return 5;
    }
    public override float GetMinimumDamagePercentage(Entity to) {
        return .25f;
    }
    public override float GetAtWallPushbackMultiplier(Entity to) {
        return cinematicHit ? 0f : 1f;
    }
    public override float GetMeterGain(Entity to, bool blocked) {
        return 0;
    }

    public override void OnHit(Entity target) {
        base.OnHit(target);
        // Debug.Log($"hit {cinematicHitWindow} {player.opponentDistance < cinematicHitDistance}");
        hasHit = true;
        if (cinematicHitWindow && player.opponentDistance < cinematicHitDistance) {
            cinematicHit = true;
        }
        
        OnSuperHit(false, cinematicHit);
    }

    public override void OnBlock(Entity target) {
        base.OnBlock(target);
        OnSuperHit(true, false);
    }

    public override Vector2 GetCarriedMomentumPercentage(Entity to) {
        return base.GetCarriedMomentumPercentage(to);
    }

    // Abstract and Virtual members
    public virtual float meterCost => 50f;
    public virtual bool hasCinematic => true;
    protected virtual int superfreezeLength => 60;
    protected virtual int superfreezeHitstopLength => 60;
    protected abstract int framesBeforeSuperfreeze { get; }
    protected abstract int farHitSkipFrame { get; }
    protected abstract int farHitActiveFrames { get; }
    protected abstract float cinematicHitDistance { get; }
    protected abstract float cinematicTotalLength { get; }

    protected virtual void OnSuperHit(bool blocked, bool cinematic) {
        if (!blocked) {
            if (cinematic) opponent.BeginState("CmnHitStunGround");
            opponent.stateFlags = cinematic ? CharacterStateFlag.TIME_SUPER_CINEMATIC : CharacterStateFlag.TIME_SUPER;
            opponent.frameData.landingFlag |= LandingRecoveryFlag.HARD_LAND_COSMETIC | LandingRecoveryFlag.HARD_KNOCKDOWN_LAND;

        } else {
            
        }
    }

    protected virtual void OnWhiff() { }

    [AnimationEventHandler("std/ReleaseCinematicSocket")]
    public virtual void OnSocketRelease(AnimationEventData data) {
        socket.Release();
        player.stateFlags = default;
        if (hasHit) {
            opponent.BeginState("CmnHitStunAir");
            // Debug.Log("hit");
            opponent.frameData.landingFlag = LandingRecoveryFlag.HARD_KNOCKDOWN_LAND | LandingRecoveryFlag.HARD_LAND_COSMETIC;
        }
    }

}
}
