using System;

namespace SuperSmashRhodes.Battle {
[Flags]
public enum AttackGuardType {
    NONE = 0,
    STANDING = 1 << 0,
    CROUCHING = 1 << 1,
    THROW = 1 << 2,
    EITHER_SIDE = 1 << 3,
    
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
    IGNORE_INVINCIBILITY = 1 << 2,
    FORCE_LAUNCH = 1 << 3,
}

[Flags]
public enum DamageSpecialProperties {
    NONE = 0,
    
    IGNORE_COMBO = 1 << 0,
    IGNORE_COMBO_DECAY = 1 << 1,
    SKIP_REGISTER = 1 << 2,
    NO_METER_GAIN = 1 << 3,
    
    REAL_DAMAGE = IGNORE_COMBO | IGNORE_COMBO_DECAY
}

[Flags]
public enum AttackAirOkType {
    NONE = 0,
    GROUND = 1 << 0,
    AIR = 1 << 1,
    ALL = GROUND | AIR
}

public enum Hitstate {
    NONE,
    COUNTER,
    PUNISH
}

[Flags]
public enum LandingRecoveryFlag {
    NONE = 0,
    UNTIL_LAND = 1 << 0,
    HARD_KNOCKDOWN_LAND = 1 << 2,
    
    HARD_LAND_COSMETIC = 1 << 3,
}

[Flags]
public enum AttackType {
    NONE = 0,
    STRIKE = 1 << 0,
    THROW = 1 << 1,
    PROJECTILE = 1 << 2,
    TOKEN = 1 << 3,
    
    FULL = STRIKE | THROW | PROJECTILE | TOKEN
}
}
