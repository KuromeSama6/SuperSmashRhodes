using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Runtime.State;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State.Implementation {
public abstract class SummonAttackStateBase : TokenState, IAttack {
    public AttackPhase phase { get; protected set; }
    public virtual bool hasActiveFrames => hitsRemaining > 0;
    public int hitsRemaining { get; protected set; }  = 1;
    public int hits { get; protected set; }
    public int blockedHits { get; protected set; }
    public PlayerCharacter player => entity.owner;
    public abstract AttackType attackType { get; }

    public SummonAttackStateBase(Entity entity) : base(entity) {
        
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        phase = AttackPhase.STARTUP;
        hitsRemaining = totalHits;
        hits = 0;
        blockedHits = 0;
        entity.boundingBoxManager.SetAll(false);
    }

    public override EntityStateSubroutine BeginMainSubroutine() {
        return Sub_Startup;
    }

    protected virtual void Sub_Startup(SubroutineContext ctx) {
        if (mainAnimation != null) {
            entity.animation.AddUnmanagedAnimation(mainAnimation, false);   
        }
        phase = AttackPhase.STARTUP;
        entity.boundingBoxManager.SetAll(false);
        OnStartup();
        
        ctx.Next(frameData.startup, Sub_Active);
    }
    
    protected virtual void Sub_Active(SubroutineContext ctx) {
        phase = AttackPhase.ACTIVE;
        entity.boundingBoxManager.SetAll(true);
        OnActive();
        
        ctx.Next(frameData.active, Sub_Recovery);
    }
    
    protected virtual void Sub_Recovery(SubroutineContext ctx) {
        phase = AttackPhase.RECOVERY;
        entity.boundingBoxManager.SetAll(false);
        OnRecovery();
        
        ctx.Next(frameData.recovery, Sub_WaitForDestroy);
    }

    protected virtual void Sub_WaitForDestroy(SubroutineContext ctx) {
        while (!mayDestroy) {
            ctx.Repeat();
        }
    }
    
    protected override void OnStateEnd(EntityState nextState) {
        base.OnStateEnd(nextState);
        entity.boundingBoxManager.SetAll(false);
        player.DestroySummon(entity);
    }

    public string GetHitSfx(Entity to) {
        return null;
    }
    public virtual void OnContact(Entity to) {
        if (hitsRemaining > 0) --hitsRemaining;
    }
    
    public virtual void OnHit(Entity target) {
        ++hits;
        // Debug.Log(player.meter.meterGainMultiplier);
        if (hits <= 1) {
            player.meter.AddMeter(GetMeterGain(target, false));
            player.meter.balance.value += .05f * GetUnscaledDamage(target) * player.meter.meterGainMultiplier;
        
            player.burst.AddDeltaTotal((GetUnscaledDamage(target) * .25f) * player.comboCounter.finalScale, 120);
            // Debug.Log($"on hit, {GetMeterGain(target, true)}");   
        }
    }
    
    public virtual void OnBlock(Entity target) {
        ++blockedHits;
        if (blockedHits <= 1) {
            player.meter.AddMeter(GetMeterGain(target, true));
            player.meter.balance.value += .02f * GetUnscaledDamage(target) * player.meter.meterGainMultiplier;
        
            player.burst.AddDeltaTotal((GetUnscaledDamage(target) * .18f) * player.comboCounter.finalScale, 240);   
        }
    }
    

    public bool MayHit(Entity target) {
        if (!hasActiveFrames) return false;
        return true;
    }
    public string GetAttackNormalSfx() {
        return null;
    }
    public string GetBlockedSfx(Entity to) {
        return null;
    }
    public AttackFrameData GetFrameData(Entity to) {
        return frameData;
    }

    public int GetCurrentFrame(Entity to) {
        return frame;
    }
    
    public float GetMeterGain(Entity to, bool blocked) {
        var attackLevel = GetAttackLevel(to);
        return (attackLevel + 1) * 1.5f * (blocked ? 1f : 2f);
    }

    // Abstract Properties
    protected abstract string mainAnimation { get; }
    public abstract AttackFrameData frameData { get; }
    
    protected virtual int totalHits => 1;
    protected virtual bool mayDestroy => true;
    
    // Events
    protected virtual void OnStartup() {
    }
    protected virtual void OnActive() {
    }
    protected virtual void OnRecovery() {
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
    public virtual float GetChipDamagePercentage(Entity to) {
        return 0.12f;
    }
    public virtual float GetOtgDamagePercentage(Entity to) {
        return 0.6f;
    }
    public abstract Vector2 GetPushback(Entity to, bool airborne, bool blocked);
    public virtual Vector2 GetCarriedMomentumPercentage(Entity to) {
        return new(0.5f, 0.5f);
    }
    public abstract float GetComboProration(Entity to);
    public virtual float GetFirstHitProration(Entity to) {
        return 1f;
    }
    public abstract AttackGuardType GetGuardType(Entity to);
    public virtual int GetFreezeFrames([CanBeNull] Entity to) {
        return GetAttackLevel(to) + 8;
    }
    public abstract int GetAttackLevel(Entity to);
    public virtual float GetMinimumDamagePercentage(Entity to) {
        return 0;
    }
    public CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.EXSMALL;
    }
    public float GetComboDecay(Entity to) {
        return 1f;
    }
    public int GetStunFrames(Entity to, bool blocked) {
        return AttackFrameData.GetStandardStun(to, blocked, GetAttackLevel(to));
    }
}

public abstract class TokenAttackStateBase : SummonAttackStateBase {
    public override EntityStateType type => EntityStateType.ENT_TOKEN;
    public override AttackType attackType => AttackType.TOKEN;
    protected TokenAttackStateBase(Entity entity) : base(entity) { }
}
}
