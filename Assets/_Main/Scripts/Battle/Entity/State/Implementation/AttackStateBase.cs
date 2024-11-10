using System.Collections;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State.Implementation {
public abstract class AttackStateBase : CharacterState {
    public AttackPhase phase { get; private set; }
    public bool hasActiveFrames => hitsRemaining > 0;
    private int hitsRemaining = 1;
    
    public AttackStateBase(Entity owner) : base(owner) {
        
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        phase = AttackPhase.STARTUP;
        hitsRemaining = 1;
        stateData.disableSideSwap = true;
    }

    public override IEnumerator MainRoutine() {
        owner.animation.AddUnmanagedAnimation(mainAnimation, false);
        phase = AttackPhase.STARTUP;
        OnStartup();
        yield return frameData.startup;

        phase = AttackPhase.ACTIVE;
        OnActive();
        yield return frameData.active;
        
        phase = AttackPhase.RECOVERY;
        OnRecovery();
        yield return frameData.recovery;
    }
    
    public virtual void OnHit(PlayerCharacter target) {
        if (hitsRemaining <= 0) return;
        --hitsRemaining;
    }
    
    // Abstract Properties
    protected abstract string mainAnimation { get; }
    public abstract AttackFrameData frameData { get; }
    public abstract AttackProperties attackProperties { get; }
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
    
}

public struct AttackFrameData {
    public int startup, active, recovery, onHit, onBlock;
    
    public int total => startup + active + recovery;
}

public struct AttackProperties {
    public float damage, chipDamagePercentage, otgDamagePercentage, pushback;
    public float comboProration, firstHitProration;
    public AttackGuardType guardType;
}
}
