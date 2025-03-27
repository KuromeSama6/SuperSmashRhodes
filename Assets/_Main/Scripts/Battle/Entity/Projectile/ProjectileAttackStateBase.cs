using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Util;
using Unity.VisualScripting;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State {
public abstract class ProjectileAttackStateBase : SummonAttackStateBase {
    public ProjectileAttackStateBase(Entity entity) : base(entity) { }
    public override AttackType attackType => AttackType.PROJECTILE;
    public override EntityStateType type => EntityStateType.ENT_TOKEN;
    public override AttackFrameData frameData => new AttackFrameData(0, lifetime, 0);

    protected override void OnStateBegin() {
        base.OnStateBegin();
        entity.animation.AddUnmanagedAnimation(mainAnimation, false);
        
        Vector2 force = new Vector2(entity.transform.right.x, entity.transform.right.y) * initialAcceleration;
        entity.rb.AddForce(force, ForceMode2D.Impulse);
    }

    protected override void OnActive() {
        base.OnActive();
    }

    protected override void OnRecovery() {
        base.OnRecovery();
    }

    protected override void OnTick() {
        base.OnTick();
        
        Vector2 force = new Vector2(entity.transform.right.x, entity.transform.right.y) * perTickAcceleration;
        entity.rb.AddForce(force, ForceMode2D.Force);
        entity.rb.linearVelocity = Vector2.Min(entity.rb.linearVelocity, terminalVelocity);
    }
    
    public override int GetAttackLevel(Entity to) {
        return projectileLevel;
    }
    public override float GetAtWallPushbackMultiplier(Entity to) {
        return 1f;
    }

    public override float GetChipDamagePercentage(Entity to) {
        return 0.05f;
    }

    //region Virtual and abstract methods
    protected abstract int lifetime { get; }
    protected abstract int projectileLevel { get; }
    protected abstract float initialAcceleration { get; }
    protected abstract float perTickAcceleration { get; }
    protected abstract Vector2 terminalVelocity { get; }
    //endregion
}
}
