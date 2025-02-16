using Spine;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Animation;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Input;
using UnityEngine;
using UnityEngine.Events;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Exusiai_MultihitWeaponAttack : State_Exusiai_FireWeaponAttack {
    protected int totalShots;
    private bool weaponDry;
    private bool shotsFired;
    private bool ended;
    
    protected UnityEvent onFireEnd { get; } = new();
    protected UnityEvent onFireStart { get; } = new();
    
    public override bool hasActiveFrames => base.hasActiveFrames && gauge.mayFire && totalShots > 0;

    public State_Exusiai_MultihitWeaponAttack(Entity entity) : base(entity) {
        
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        totalShots = maxHits;
        weaponDry = false;
        shotsFired = false;
        ended = false;
    }

    protected override void OnActive() {
        base.OnActive();
        if (gauge.mayFire) {
            entity.audioManager.PlaySound(weaponSfx);
            
        } else {
            FastForward(emptySkipFrame - frame);
        }
    }

    protected override void OnRecovery() {
        base.OnRecovery();
        if (shotsFired) {
            entity.audioManager.PlaySound($"chr/exusiai/battle/sfx/gun_shell_{Random.Range(1, 4)}");
        }
    }

    [AnimationEventHandler("FireWeapon")]
    public virtual void OnFireWeapon(AnimationEventData args) {
        if (totalShots <= 0) {
            return;
        }
        if (!gauge.mayFire) {
            if (!weaponDry) {
                entity.audioManager.PlaySound("chr/exusiai/battle/sfx/gun_empty");
                weaponDry = true;   
            }
            if (!ended) {
                ended = true;
                onFireEnd.Invoke();
            }
            return;
        }
        
        gauge.Fire(true, false);
        --totalShots;

        if (!shotsFired) {
            onFireStart.Invoke();
        }
        
        shotsFired = true;
        hitsRemaining += 1;
        
        if (totalShots <= 0) {
            ended = true;
            onFireEnd.Invoke();
        }
    }
    
    protected abstract int maxHits { get; }
    protected abstract int emptySkipFrame { get; }
    protected abstract string weaponSfx { get; }

    public override string GetAttackNormalSfx() {
        return null;
    }
    public override float GetChipDamagePercentage(Entity to) {
        return 0f;
    }
}
}
