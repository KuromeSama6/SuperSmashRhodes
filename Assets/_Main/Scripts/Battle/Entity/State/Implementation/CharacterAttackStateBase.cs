using System.Collections;
using JetBrains.Annotations;
using SuperSmashRhodes.Input;
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
    }
    
    public virtual void OnBlock(Entity target) {
        player.audioManager.PlaySound(GetBlockedSfx(target), .4f);
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
            frames = 6;
        }

        return buffer.TimeSlice(frames).ScanForInput(input); 
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
        // Debug.Log($"onactive addcancel {commonCancelOptions}");
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
    public abstract int GetFreezeFrames([CanBeNull] Entity to);
    public abstract int GetAttackLevel(Entity to);
    public abstract string GetAttackNormalSfx();
    protected abstract InputFrame[] requiredInput { get; }

    public string GetHitSfx(Entity to) {
        return "battle_generic_hit1";
    }
    public string GetBlockedSfx(Entity to) {
        return "battle_generic_block2";
    }
}

}
