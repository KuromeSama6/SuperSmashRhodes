using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Runtime.Gauge;
using SuperSmashRhodes.Runtime.Tokens;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.Character {
/// <summary>
/// Sword summoner (projectile attack).
/// </summary>
public abstract class State_Rosmontis_DriveAttack : CharacterAttackStateBase {
    public State_Rosmontis_DriveAttack(Entity entity) : base(entity) { }
    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_DRIVE_SPECIAL_SUPER;
    protected override int normalInputBufferLength => 6;
    protected override float inputMeter => 0;
    public override bool mayEnterState => base.mayEnterState && swordManager.hasFreeSwords && swordManager.power.value >= 1;

    protected Token_Rosmontis_Sword dispatchedSword { get; private set; }
    protected Gauge_Rosmontis_SwordManager swordManager => player.GetComponent<Gauge_Rosmontis_SwordManager>();
    protected bool hasProjectile { get; private set; }
    public override Hitstate hitstate => Hitstate.COUNTER;
    protected abstract Vector2 summonOffset { get; }
    
    private Vector2 previousVelocity;

    protected override void OnStartup() {
        base.OnStartup();
        dispatchedSword = swordManager.GetDispatchedSword("DriveAttack");
        if (dispatchedSword == null) {
            dispatchedSword = swordManager.DispatchAny(1, "DriveAttack");
            entity.PlaySound("chr/rosmontis/battle/sfx/sword/dispatch/medium");
        } else {
            swordManager.UsePower(1);
        }

        hasProjectile = dispatchedSword;
        previousVelocity = Vector2.zero;
    }

    protected override void OnActive() {
        base.OnActive();
        entity.PlaySound("chr/rosmontis/battle/sfx/drive", .5f);
        
        // summon projectile
        var sword = player.Summon<Token_Rosmontis_ThrownSword>("chr/rosmontis/battle/token/thrownsword", summonOffset);
        sword.InitSword(projectileState);
        
        if (player.airborne) {
            previousVelocity = player.rb.linearVelocity;
            player.rb.linearVelocity = Vector2.zero;
            stateData.gravityScale = 0;
        }
    }

    protected override void OnRecovery() {
        base.OnRecovery();
        
        if (player.airborne) {
            player.rb.linearVelocityY = Mathf.Min(-7, previousVelocity.y);
        }
    }

    protected override void OnStateEnd(EntityState nextState) {
        base.OnStateEnd(nextState);
        if (nextState is State_Rosmontis_DriveAttack) return;

        if (dispatchedSword) {
            dispatchedSword.Release();
        }
    }

    public override float GetUnscaledDamage(Entity to) {
        return 0;
    }
    public override float GetChipDamagePercentage(Entity to) {
        return 0;
    }
    public override float GetOtgDamagePercentage(Entity to) {
        return 0;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return Vector2.zero;
    }
    public override float GetComboProration(Entity to) {
        return 0;
    }
    public override float GetFirstHitProration(Entity to) {
        return 0;
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override int GetFreezeFrames(Entity to) {
        return 0;
    }
    public override int GetAttackLevel(Entity to) {
        return 0;
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.EXSMALL;
    }
    
    protected abstract string projectileState { get; }
}
}
