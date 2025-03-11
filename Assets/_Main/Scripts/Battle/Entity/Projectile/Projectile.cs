using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.State;

namespace SuperSmashRhodes.Battle {
public abstract class Projectile : Token {
    public override TokenFlag flags => base.flags | TokenFlag.DESTROY_ON_OWNER_DAMAGE;

    protected override void Start() {
        base.Start();

        rb.gravityScale = 0f;
    }

    public override void EngineUpdate() {
        base.EngineUpdate();
    }

    protected override IAttack OnOutboundHit(Entity victim, EntityBBInteractionData data) {
        base.OnOutboundHit(victim, data);
        
        if (victim is PlayerCharacter player) {
            return ValidateOutboundHit(player);
        }
        
        //TODO: Others 
        return null;
    }

    private IAttack ValidateOutboundHit(PlayerCharacter to) {
        if (!(activeState is ProjectileAttackStateBase move)) {
            // invalid attack state1
            return null;
        }
        
        // reject if move has no active frames
        if (!move.hasActiveFrames) return null;
        // Debug.Log($"move {move.phase} {move.frame}"); 
        // reject if move is not active
        if (move.phase != AttackPhase.ACTIVE) return null;
        // move.OnContact(to);
        return move;
    }
}
}
