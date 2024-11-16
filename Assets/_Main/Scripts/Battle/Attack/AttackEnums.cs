using System;

namespace SuperSmashRhodes.Battle {
[Flags]
public enum AttackGuardType {
    NONE = 0,
    STANDING = 1 << 0,
    CROUCHING = 1 << 1,
    THROW = 1 << 2,
    
    ALL = STANDING | CROUCHING
}

public enum AttackResult {
    PENDING,
    HIT,
    BLOCKED
}

public enum AttackPhase {
    STARTUP,
    ACTIVE,
    RECOVERY,
}

[Flags]
public enum AttackSpecialProperties {
    NONE = 0,
    SOFT_KNOCKDOWN = 1 << 0,
    HARD_KNOCKDOWN = 1 << 1,
}
}
