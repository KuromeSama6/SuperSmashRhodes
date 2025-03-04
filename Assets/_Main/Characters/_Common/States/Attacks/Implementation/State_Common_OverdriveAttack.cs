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
    public override bool mayEnterState => player.meter.gauge.value >= meterCost;
    public override CharacterStateFlag globalFlags => CharacterStateFlag.PAUSE_GAUGE | CharacterStateFlag.GLOBAL_PAUSE_TIMER;
    public override StateIndicatorFlag stateIndicator => StateIndicatorFlag.SUPER | (player.burst.driveRelease ? StateIndicatorFlag.DRIVE_RELEASE : StateIndicatorFlag.NONE) | (cinematicHit ? StateIndicatorFlag.INVINCIBLE : StateIndicatorFlag.NONE);
    public override bool landCancellable => false;


    protected CinematicCharacterSocket socket { get; private set; }
    protected bool cinematicHit { get; private set; }
    protected bool hasHit { get; private set; }
    protected bool cinematicHitWindow { get; private set; }
    protected bool isDriveReleaseCancel { get; private set; }
    protected virtual bool useCinematicSocket => true;
    protected virtual EntityGhostFXData ghostFXData => new(Color.white);
    protected virtual Color cinematicBackgroundColor => "2B395C".HexToColor();

    private int activeFrameCounter;
    
    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.meter.gauge.value -= meterCost;
        cinematicHit = false;
        hasHit = false;
        cinematicHitWindow = true;
        isDriveReleaseCancel = player.burst.driveRelease;

        stateData.cameraData.cameraWeightModifier = 2f;
        stateData.cameraData.cameraFovModifier = -5f;
        
        if (socket != null) {
            socket.Release();
            socket = null;   
        }
        
        player.fxManager.PlayEmblemFX(1f, true);
        activeFrameCounter = 0;
    }
    
    public override EntityStateSubroutine BeginMainSubroutine() {
        return Sub_StartupBeforeSuperfreeze;
    }

    //region Subroutines
    protected virtual void Sub_StartupBeforeSuperfreeze(SubroutineContext ctx) {
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
        
        ctx.Next(framesBeforeSuperfreeze, Sub_StartupAfterSuperfreeze);
    }
    
    protected virtual void Sub_StartupAfterSuperfreeze(SubroutineContext ctx) {
        TimeManager.inst.globalFreezeFrames = superfreezeLength;
        ctx.Next(frameData.startup - framesBeforeSuperfreeze + 1, Sub_ActiveNormal);
    }

    protected virtual void Sub_ActiveNormal(SubroutineContext ctx) {
        phase = AttackPhase.ACTIVE;
        stateData.cameraData.cameraFovModifier = 0f; 
        stateData.cameraData.cameraWeightModifier = 0f;
        OnActive();
        player.ApplyGroundedFriction(frameData.active);
        entity.audioManager.PlaySound(GetAttackNormalSfx());
        cinematicHitWindow = true;
        
        ctx.Next(0, Sub_ActiveCinematicCheckLoop);
    }
    
    protected virtual void Sub_ActiveCinematicCheckLoop(SubroutineContext ctx) {
        if (cinematicHit) {
            ctx.Next(0, Sub_ActiveCinematic);
            return;
        }
        
        if (activeFrameCounter < frameData.active) {
            ctx.Repeat();
            ++activeFrameCounter;
            return;
        }

        cinematicHitWindow = false;
        ctx.Next(0, Sub_ActiveNonCinematic);
    }

    protected virtual void Sub_ActiveCinematic(SubroutineContext ctx) {
        // big cinematic effect!!!
        stateData.gravityScale = 0;
        stateData.ghostFXData = ghostFXData;

        if (useCinematicSocket) {
            socket = new CinematicCharacterSocket(opponent, player, "throw_opponent", new(0, 0, 0));
            socket.Attach();   
        }
            
        // superfreeze part 2
        stateData.backgroundUIData.dimAlpha = 1f;
        stateData.backgroundUIData.bgType = BackgroundType.SUPER;
        stateData.backgroundUIData.bgColor = cinematicBackgroundColor;
        stateData.backgroundUIData.transitionFrame = 0;
        stateData.backgroundUIData.transition = TransitionType.SUPER_FADE_IN;

        TimeManager.inst.globalFreezeFrames = superfreezeHitstopLength;

        var length = cinematicTotalLength - frameData.active - frameData.startup + 1;
        ctx.Exit((int)length);
    }
    
    protected virtual void Sub_ActiveNonCinematic(SubroutineContext ctx) {
        if (hitsRemaining > 0) OnWhiff();
        player.animation.Tick(farHitSkipFrame - frame);
        opponent.stateFlags = player.stateFlags = default;
        stateData.backgroundUIData = new();
        
        ctx.Next(farHitActiveFrames, Sub_RecoveryStart);
    }
    
    
    //endregion
    
    protected override void OnStateEnd(EntityState nextState) {
        base.OnStateEnd(nextState);
        if (socket != null && socket.attached) {
            socket.Release();
            opponent.stateFlags = player.stateFlags = default;
        }

        player.burst.releaseFrames = 0;
        opponent.stateFlags = player.stateFlags = default;
        player.meter.penaltyFrames += 180;
    }

    protected override void OnTick() {
        base.OnTick();
        if (socket != null && socket.attached) socket.Tick();
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
        return isDriveReleaseCancel ? .5f : .25f;
    }
    public override float GetAtWallPushbackMultiplier(Entity to) {
        return cinematicHit ? 0f : 1f;
    }
    public override float GetMeterGain(Entity to, bool blocked) {
        return 0;
    }

    public override void OnHit(Entity target) {
        base.OnHit(target);
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
            if (cinematic) {
                opponent.BeginState("CmnHitStunGround");
            }
            opponent.stateFlags = cinematic ? CharacterStateFlag.TIME_SUPER_CINEMATIC : CharacterStateFlag.TIME_SUPER;
            opponent.frameData.landingFlag |= LandingRecoveryFlag.HARD_LAND_COSMETIC | LandingRecoveryFlag.HARD_KNOCKDOWN_LAND;

        } else {
            
        }
    }

    protected virtual void OnWhiff() { }
    protected virtual void OnRelease() { }

    public override void OnApplyCinematicDamage(AnimationEventData data) {
        if (!cinematicHit) return;
        base.OnApplyCinematicDamage(data);
    }

    protected void ReleaseSocket() {
        if (socket == null || !socket.attached) return;
        
        if (socket != null && socket.attached) {
            OnRelease();
        }
        
        if (socket != null) socket.Release();
        
        player.stateFlags = default;
        opponent.stateFlags = player.stateFlags = default;
        stateData.backgroundUIData = new();
        
        if (hasHit) {
            opponent.BeginState("CmnHitStunAir");
            opponent.ApplyForwardVelocity(new(0, 3));
            opponent.frameData.landingFlag = LandingRecoveryFlag.HARD_KNOCKDOWN_LAND | LandingRecoveryFlag.HARD_LAND_COSMETIC;
        }
    }
    
    [AnimationEventHandler("std/ReleaseCinematicSocket")]
    public virtual void OnSocketRelease(AnimationEventData data) {
        ReleaseSocket();
    }

}
}
