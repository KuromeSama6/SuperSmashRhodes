using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SuperSmashRhodes.Battle.Animation;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Runtime.State;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State.Implementation {
public abstract class CharacterAttackStateBase : CharacterState, IAttack {
    public AttackPhase phase { get; protected set; }
    public virtual bool hasActiveFrames => hitsRemaining > 0 && !hitLock; 
    public int hitsRemaining { get; protected set; }  = 1;
    public int hits { get; protected set; }
    public int blockedHits { get; protected set; }
    public bool hitLock { get; protected set; }
    public int attackStage { get; private set; }
    
    public virtual AttackType attackType => AttackType.STRIKE;
    public override bool mayEnterState => player.MatchesAirState(airOk);
    public override Hitstate hitstate => phase == AttackPhase.RECOVERY ? Hitstate.PUNISH : Hitstate.COUNTER;
    public bool chargable => this is IChargable;
    public virtual bool landCancellable => true;

    public CharacterAttackStateBase(Entity entity) : base(entity) {
        
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        phase = AttackPhase.STARTUP;
        hitsRemaining = totalHits;
        stateData.disableSideSwap = true;
        player.SetZPriority();
        TimeManager.inst.globalFreezeFrames = 0;
        hits = 0;
        blockedHits = 0;
        chargeLevel = 0;
        hitLock = false;
        attackStage = 0;
    }

    public override EntityStateSubroutine BeginMainSubroutine() {
        return Sub_Startup;
    }
    
    protected virtual void Sub_Startup(SubroutineContext ctx) {
        entity.animation.AddUnmanagedAnimation(mainAnimation, false);
        phase = AttackPhase.STARTUP;
        OnStartup();
        
        player.ApplyGroundedFriction(frameData.startup);
        entity.audioManager.PlaySound(GetAttackNormalSfx());

        ctx.Next(frameData.startup, Sub_Active);
    }
    
    protected virtual void Sub_Active(SubroutineContext ctx) {
        phase = AttackPhase.ACTIVE;
        OnActive();
        player.ApplyGroundedFriction(frameData.active);
        ctx.Next(frameData.active, Sub_RecoveryStart);
    }

    protected virtual void Sub_RecoveryStart(SubroutineContext ctx) {
        player.ApplyGroundedFriction(frameData.active);
        phase = AttackPhase.RECOVERY;
        OnRecovery();

        if (player.airborne && player.frameData.landingFlag.HasFlag(LandingRecoveryFlag.UNTIL_LAND)) {
            player.frameData.landingRecoveryFrames = frameData.recovery;
            ctx.Next(0, Sub_GroundedRecovery);
            
        } else {
            ctx.Exit(frameData.recovery);
        }
    }

    protected virtual void Sub_GroundedRecovery(SubroutineContext ctx) {
        if (player.airborne) {
            ctx.Repeat(1);
        } else {
            CancelInto("CmnLandingRecovery");
        }
    }
    
    protected override void OnStateEnd(EntityState nextState) {
        base.OnStateEnd(nextState);
        player.boundingBoxManager.SetAll(false);
    }

    public virtual void OnContact(Entity to) {
        if (hitsRemaining > 0) --hitsRemaining;
        if (shouldSetHitLock) hitLock = true;
        // Debug.Log("cancel added"); 
        AddCancelOption(commonCancelOptions);
    }

    protected override void OnTick() {
        base.OnTick();
        // Debug.Log($"tick {phase} {frame} {Time.frameCount} ani {player.animation.animation.AnimationState.GetCurrent(0).AnimationTime / Time.fixedDeltaTime} ");
    }

    public virtual void OnHit(Entity target) {
        ++hits;
        player.audioManager.PlaySound(GetHitSfx(target), .6f);
        // Debug.Log(player.meter.meterGainMultiplier);
        if (hits <= 1) {
            player.meter.AddMeter(GetMeterGain(target, false));
            player.meter.balance.value += .05f * GetUnscaledDamage(target) * player.meter.meterBalanceMultiplier;
        
            player.burst.AddDeltaTotal((GetUnscaledDamage(target) * .25f) * player.comboCounter.finalScale, 120);
            // Debug.Log($"on hit, {GetMeterGain(target, true)}");   
        }
    }
    
    public virtual void OnBlock(Entity target) {
        ++blockedHits;
        player.audioManager.PlaySound(GetBlockedSfx(target), .4f);

        if (blockedHits <= 1) {
            player.meter.AddMeter(GetMeterGain(target, true));
            player.meter.balance.value += .02f * GetUnscaledDamage(target) * player.meter.meterGainMultiplier;;
        
            player.burst.AddDeltaTotal((GetUnscaledDamage(target) * .18f) * player.comboCounter.finalScale, 240);   
        }
    }

    public bool MayHit(Entity target) {
        if (!hasActiveFrames) return false;
        return true;
    }

    public AttackFrameData GetFrameData(Entity to) {
        return frameData;
    }
    
    public AttackData CreateAttackData(Entity to) {
        return new AttackData() {
            interactionData = default,
            attack = this,
            from = player,
            to = to,
            result = AttackResult.HIT
        };
    }
    
    public int GetCurrentFrame(Entity to) {
        return frame;
    }
    
    public override bool IsInputValid(InputBuffer buffer) {
        var input = requiredInput;
        if (input == null) return false;
        
        int frames = normalInputBufferLength;
        // if (entity.activeState is CharacterAttackStateBase attack) {
        //     // Debug.Log(attack.frameData.startup + frameData.startup + frameData.active);
        //     frames = Mathf.Max(1, attack.GetFreezeFrames(null) + frameData.startup + frameData.active);
        // } else {
        //     frames = normalInputBufferLength;
        // }

        return buffer.TimeSlice(frames).ScanForInput(entity.side, input); 
    }
    
    public virtual float GetMeterGain(Entity to, bool blocked) {
        var attackLevel = GetAttackLevel(to);
        return (attackLevel + 1) * 1f * (blocked ? 1f : 2f) * player.comboCounter.finalScale;
    }

    // Abstract Properties
    protected abstract string mainAnimation { get; }
    public abstract AttackFrameData frameData { get; }
    protected abstract EntityStateType commonCancelOptions { get; }
    protected virtual InputFrame[] requiredInput => null;
    protected abstract int normalInputBufferLength { get; }
    protected abstract float inputMeter { get; }
    
    protected virtual int totalHits => 1;
    protected virtual AttackAirOkType airOk => AttackAirOkType.GROUND;
    public virtual LandingRecoveryFlag landingRecoveryFlag => LandingRecoveryFlag.NONE;
    protected virtual bool shouldSetHitLock => true;
    
    // Events
    protected virtual void OnStartup() {
        player.meter.AddMeter(inputMeter); 
    }
    protected virtual void OnActive() {
        // Debug.Log($"onactive addcancel {commonCancelOptions}");
        AddCancelOption("CmnWhiteForceReset");
        AddCancelOption("CmnDriveRelease");
    }
    protected virtual void OnRecovery() {
        player.frameData.landingFlag = landingRecoveryFlag;
    }

    // Member Methods
    public virtual AttackSpecialProperties GetSpecialProperties(Entity to) {
        return AttackSpecialProperties.NONE;
    }
    public virtual DamageProperties GetDamageSpecialProperties(Entity to) {
        var ret = DamageProperties.NONE;
        if (hits > 1) ret |= DamageProperties.NO_METER_GAIN;
        return ret;
    }

    public virtual float GetComboDecayIncreaseMultiplier(Entity to) {
        return 1f;
    }
    public virtual float GetAtWallPushbackMultiplier(Entity to) {
        return 1f;
    }

    public abstract float GetUnscaledDamage(Entity to);
    public abstract float GetChipDamagePercentage(Entity to);
    public abstract float GetOtgDamagePercentage(Entity to);
    public abstract Vector2 GetPushback(Entity to, bool airborne, bool blocked);
    public virtual Vector2 GetCarriedMomentumPercentage(Entity to) {
        return new(0.5f, 0.5f);
    }
    public abstract float GetComboProration(Entity to);
    public abstract float GetFirstHitProration(Entity to);
    public abstract AttackGuardType GetGuardType(Entity to);
    public abstract CounterHitType GetCounterHitType(Entity to);
    public abstract int GetFreezeFrames([CanBeNull] Entity to);
    public abstract int GetAttackLevel(Entity to);
    public virtual float GetMinimumDamagePercentage(Entity to) {
        return 0;
    }
    public virtual string GetAttackNormalSfx() {
        return null;
    }
    protected virtual void OnNotifyStage(int stage) {}
    
    public virtual int GetExtraStunFrames(Entity to, bool blocked) {
        return 0;
    }

    public string GetHitSfx(Entity to) {
        return "cmn/battle/sfx/hit/1";
    }
    public string GetBlockedSfx(Entity to) {
        return "cmn/battle/sfx/block/2";
    }
    public float GetComboDecay(Entity to) {
        return 1f;
    }
    
    public virtual int GetStunFrames(Entity to, bool blocked) {
        // return Mathf.Max(0, frameData.active + frameData.recovery + GetFrameAdvantage(to, blocked) - (frame - frameData.startup));
        // standardized hitstun
        
        var extra = GetExtraStunFrames(to, blocked);
        return AttackFrameData.GetStandardStun(to, blocked, GetAttackLevel(to)) + extra;
    }
    
    public override void OnLand(LandingRecoveryFlag flag, int recoveryFrames) {
        base.OnLand(flag, recoveryFrames);
        // Debug.Log($"land, this {id}, flag {flag}, recov {recoveryFrames}, this flag {landingRecoveryFlag.HasFlag(LandingRecoveryFlag.CARRY_CANCEL_OPTIONS)}, fd recov {player.frameData.landingRecoveryFrames}");
        if (flag.HasFlag(LandingRecoveryFlag.NO_LANDING_RECOVERY) || !landCancellable) {
            // Debug.Log("no recov");
            return;
        }

        if (landingRecoveryFlag.HasFlag(LandingRecoveryFlag.UNTIL_LAND)) {
            player.frameData.landingFlag |= LandingRecoveryFlag.UNTIL_LAND;
            player.frameData.landingRecoveryFrames = frameData.recovery;
            // Debug.Log("until land");
        } else {
            player.frameData.landingRecoveryFrames = 3;   
        }

        if (landingRecoveryFlag.HasFlag(LandingRecoveryFlag.CARRY_CANCEL_OPTIONS)) {
            player.SetCarriedStateVariable("_carriedCancelFlags", "CmnLandingRecovery", stateData.cancelFlag);
        }
        
        CancelInto("CmnLandingRecovery");
    }

    [AnimationEventHandler("std/ClearHitLock")]
    public virtual void OnClearHitLock(AnimationEventData data) {
        hitLock = false;
        ++attackStage;
        OnNotifyStage(attackStage);
        // Debug.Log($"clear hit lock {attackStage}");
    }

    [AnimationEventHandler("std/NotifyStage")]
    public virtual void OnNotifyStage(AnimationEventData data) {
        ++attackStage;
        OnNotifyStage(attackStage);
    }
    
    
    [AnimationEventHandler("std/ApplyCinematicDamage")]
    public virtual void OnApplyCinematicDamage(AnimationEventData data) {
        player.opponent.ApplyDamage(data.integerValue, CreateAttackData(player.opponent), DamageProperties.SKIP_REGISTER | DamageProperties.IGNORE_COMBO_DECAY); 
    }
}
}
