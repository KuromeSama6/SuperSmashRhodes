using System.Collections;
using JetBrains.Annotations;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State.Implementation {
public abstract class CharacterAttackStateBase : CharacterState, IAttack {
    public AttackPhase phase { get; protected set; }
    public virtual bool hasActiveFrames => hitsRemaining > 0; 
    public int hitsRemaining { get; protected set; }  = 1;
    public int hits { get; protected set; }
    public int blockedHits { get; protected set; }
    
    public virtual AttackType attackType => AttackType.STRIKE;
    public override bool mayEnterState => player.MatchesAirState(airOk);
    public override Hitstate hitstate => phase == AttackPhase.RECOVERY ? Hitstate.PUNISH : Hitstate.COUNTER;
    public bool chargable => this is IChargable;

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
    }

    public override IEnumerator MainRoutine() {
        entity.animation.AddUnmanagedAnimation(mainAnimation, false);
        // Debug.Log("start");
        phase = AttackPhase.STARTUP;
        OnStartup();
        
        player.ApplyGroundedFriction(frameData.startup);
        entity.audioManager.PlaySound(GetAttackNormalSfx());
        yield return frameData.startup;
        // Debug.Log($"active {frame} {Time.frameCount}");

        phase = AttackPhase.ACTIVE;
        OnActive();
        player.ApplyGroundedFriction(frameData.active);
        yield return frameData.active;
        
        player.ApplyGroundedFriction(frameData.active);
        // Debug.Log($"recov {frame}");
        
        // Debug.Log($"rec {frame} {Time.frameCount}");
        phase = AttackPhase.RECOVERY;
        OnRecovery();
        // Debug.Log($"{id} {player.frameData.landingFlag} {player.airborne}");
        if (player.airborne && player.frameData.landingFlag.HasFlag(LandingRecoveryFlag.UNTIL_LAND)) {
            player.frameData.landingRecoveryFrames = frameData.recovery;
            while (player.airborne) {
                // Debug.Log("wait");
                yield return 1;
            }
            CancelInto("CmnLandingRecovery");
            
        } else {
            yield return frameData.recovery;   
        }
    }

    protected override void OnStateEnd(string nextState) {
        base.OnStateEnd(nextState);
        player.boundingBoxManager.SetAll(false);
    }

    public virtual void OnContact(Entity to) {
        if (hitsRemaining > 0) --hitsRemaining;
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
        
        int frames;
        if (entity.activeState is CharacterAttackStateBase attack) {
            // Debug.Log(attack.frameData.startup + frameData.startup + frameData.active);
            frames = Mathf.Max(1, attack.GetFreezeFrames(null) + frameData.startup + frameData.active);
        } else {
            frames = normalInputBufferLength;
        }

        return buffer.TimeSlice(frames).ScanForInput(entity.side, input); 
    }
    
    public virtual float GetMeterGain(Entity to, bool blocked) {
        var attackLevel = GetAttackLevel(to);
        return (attackLevel + 1) * 1.5f * (blocked ? 1f : 2f) * player.comboCounter.finalScale;
    }

    // Abstract Properties
    protected abstract string mainAnimation { get; }
    public abstract AttackFrameData frameData { get; }
    protected abstract EntityStateType commonCancelOptions { get; }
    protected abstract InputFrame[] requiredInput { get; }
    protected abstract int normalInputBufferLength { get; }
    protected abstract float inputMeter { get; }
    
    protected virtual int totalHits => 1;
    protected virtual AttackAirOkType airOk => AttackAirOkType.GROUND;
    public virtual LandingRecoveryFlag landingRecoveryFlag => LandingRecoveryFlag.NONE;
    
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
    public virtual int GetFrameAdvantage(Entity to, bool blocked) {
        var data = frameData;
        return blocked ? data.onBlock : data.onHit;
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
    
    public int GetStunFrames(Entity to, bool blocked) {
        return frameData.active + frameData.recovery + GetFrameAdvantage(to, blocked);
    }
    
    public override void OnLand(LandingRecoveryFlag flag, int recoveryFrames) {
        base.OnLand(flag, recoveryFrames);
        if (landingRecoveryFlag.HasFlag(LandingRecoveryFlag.UNTIL_LAND)) {
            player.frameData.landingFlag |= LandingRecoveryFlag.UNTIL_LAND;
            player.frameData.landingRecoveryFrames = frameData.recovery;
        } else {
            player.frameData.landingRecoveryFrames = 3;   
        }
        CancelInto("CmnLandingRecovery");
    }
}
}
