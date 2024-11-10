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

public enum AttackPhase {
    STARTUP,
    ACTIVE,
    RECOVERY,
}
}
