using System.Collections;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State.Implementation {
public abstract class CharacterAttackStateBase : CharacterState, IAttack {
    public AttackPhase phase { get; private set; }
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

    public virtual void OnContact(PlayerCharacter to) {
    }
    
    public virtual void OnHit(Entity target) {
        if (hitsRemaining <= 0) return;
        player.audioManager.PlaySound(GetHitSfx(target), .6f);
    }
    
    public virtual void OnBlock(Entity target) {
        player.audioManager.PlaySound(GetBlockedSfx(target), .4f);
    }

    public void OnHitProcessed(Entity to) {
        if (hitsRemaining > 0) --hitsRemaining;
    }

    public bool MayHit(Entity target) {
        if (phase != AttackPhase.ACTIVE) return false;
        if (!hasActiveFrames) return false;
        return true;
    }

    public AttackFrameData GetFrameData(Entity to) {
        return frameData;
    }

    public int GetCurrentFrame(Entity to) {
        return frame;
    }

    // Abstract Properties
    protected abstract string mainAnimation { get; }
    public abstract AttackFrameData frameData { get; }
    protected abstract EntityStateType commonCancelOptions { get; }
    protected virtual int totalHits => 1;
    
    // Events
    protected virtual void OnStartup() {
    }
    protected virtual void OnActive() {
        AddCancelOption(commonCancelOptions);
    }
    protected virtual void OnRecovery() {
    }
    
    // Member Methods

    public abstract float GetUnscaledDamage(Entity to);
    public abstract float GetChipDamagePercentage(Entity to);
    public abstract float GetOtgDamagePercentage(Entity to);
    public abstract Vector2 GetPushback(Entity to, bool airborne);
    public abstract float GetComboProration(Entity to);
    public abstract float GetFirstHitProration(Entity to);
    public abstract AttackGuardType GetGuardType(Entity to);
    public abstract int GetFreezeFrames(Entity to);
    public abstract int GetAttackLevel(Entity to);
    public abstract string GetAttackNormalSfx();

    public string GetHitSfx(Entity to) {
        return "battle_generic_hit1";
    }
    public string GetBlockedSfx(Entity to) {
        return "battle_generic_block2";
    }
}

}
