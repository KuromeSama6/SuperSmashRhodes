using UnityEngine;

namespace SuperSmashRhodes.Battle {
public struct StandardAttackResult {
    public EntityBBInteractionData interactionData;
    public IAttack attack;
    public Entity from;
    public Entity to;
    public AttackResult result;
}

public enum AttackResult {
    HIT,
    BLOCKED
}

public struct AttackFrameData {
    public int startup, active, recovery, onHit, onBlock;
    public int total => startup + active + recovery;
}
}
