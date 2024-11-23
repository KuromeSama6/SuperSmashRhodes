using System.Collections;
using JetBrains.Annotations;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State.Implementation {
public abstract class CharacterAttackStateBase : CharacterState, IAttack {
    public AttackPhase phase { get; protected set; }
    public bool hasActiveFrames => hitsRemaining > 0;
    private int hitsRemaining = 1;
    
    public CharacterAttackStateBase(Entity owner) : base(owner) {
        
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        phase = AttackPhase.STARTUP;
        hitsRemaining = 1;
        stateData.disableSideSwap = true;
        player.SetZPriority();
        PhysicsTickManager.inst.globalFreezeFrames = 0; 
    }

    public override IEnumerator MainRoutine() {
        owner.animation.AddUnmanagedAnimation(mainAnimation, false);
        phase = AttackPhase.STARTUP;
        OnStartup();
        
        player.ApplyGroundedFriction(frameData.startup);
        owner.audioManager.PlaySound(GetAttackNormalSfx());
        yield return frameData.startup;

        phase = AttackPhase.ACTIVE;
        OnActive();
        player.ApplyGroundedFriction(frameData.active);
        yield return frameData.active;
        
        player.ApplyGroundedFriction(frameData.active);
        phase = AttackPhase.RECOVERY;
        OnRecovery();
        yield return frameData.recovery;
    }

    protected override void OnStateEnd() {
        base.OnStateEnd();
        player.boundingBoxManager.DisableAll();
    }

    public virtual void OnContact(Entity to) {
        if (hitsRemaining > 0) --hitsRemaining;
        // Debug.Log("cancel added"); 
        AddCancelOption(commonCancelOptions);
    }
    
    public virtual void OnHit(Entity target) {
        player.audioManager.PlaySound(GetHitSfx(target), .6f);
        // Debug.Log(player.meter.meterGainMultiplier);
        player.meter.gauge.value += GetMeterGain(target, false) * player.meter.meterGainMultiplier;
        player.meter.balance.value += .05f * GetUnscaledDamage(target) * player.meter.meterGainMultiplier;
        
        player.burst.AddDeltaTotal((GetUnscaledDamage(target) * .25f) * player.comboCounter.finalScale, 120);
        // Debug.Log($"on hit, {GetMeterGain(target, true)}");
    }
    
    public virtual void OnBlock(Entity target) {
        player.audioManager.PlaySound(GetBlockedSfx(target), .4f);
        player.meter.gauge.value += GetMeterGain(target, true) * player.meter.meterGainMultiplier;;
        player.meter.balance.value += .02f * GetUnscaledDamage(target) * player.meter.meterGainMultiplier;;
        
        player.burst.AddDeltaTotal((GetUnscaledDamage(target) * .18f) * player.comboCounter.finalScale, 240);
    }
    

    public bool MayHit(Entity target) {
        if (!hasActiveFrames) return false;
        return true;
    }

    public AttackFrameData GetFrameData(Entity to) {
        return frameData;
    }

    public int GetCurrentFrame(Entity to) {
        return frame;
    }

    public override bool IsInputValid(InputBuffer buffer) {
        var input = requiredInput;
        int frames;
        if (owner.activeState is CharacterAttackStateBase attack) {
            // Debug.Log(attack.frameData.startup + frameData.startup + frameData.active);
            frames = Mathf.Max(1, attack.GetFreezeFrames(null) + frameData.startup + frameData.active);
        } else {
            frames = normalInputBufferLength;
        }

        return buffer.TimeSlice(frames).ScanForInput(input); 
    }
    
    public float GetMeterGain(Entity to, bool blocked) {
        var attackLevel = GetAttackLevel(to);
        return (attackLevel + 1) * 1.5f * (blocked ? 1f : 2f);
    }

    // Abstract Properties
    protected abstract string mainAnimation { get; }
    public abstract AttackFrameData frameData { get; }
    protected abstract EntityStateType commonCancelOptions { get; }
    protected abstract InputFrame[] requiredInput { get; }
    protected abstract int normalInputBufferLength { get; }
    
    protected virtual int totalHits => 1;
    
    // Events
    protected virtual void OnStartup() {
    }
    protected virtual void OnActive() {
        // Debug.Log($"onactive addcancel {commonCancelOptions}");
        AddCancelOption("CmnWhiteForceReset");
    }
    protected virtual void OnRecovery() {
    }

    // Member Methods
    public virtual AttackSpecialProperties GetSpecialProperties(Entity to) {
        return AttackSpecialProperties.NONE;
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
    public abstract int GetFreezeFrames([CanBeNull] Entity to);
    public abstract int GetAttackLevel(Entity to);
    public virtual string GetAttackNormalSfx() {
        return null;
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
}
}
