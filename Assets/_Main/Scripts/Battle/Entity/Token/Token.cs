using System;
using SuperSmashRhodes.Battle.State;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
/**
 * Tokens are things summoned by players, either explicitly through a move, or automatically by the system.
 */
public abstract class Token : Entity {
    public virtual TokenFlag flags => TokenFlag.NONE;
}

[Flags]
public enum TokenFlag : ulong {
    NONE = 0,
    
    INTERACTABLE = 1 << 0,
    DESTROY_ON_OWNER_DAMAGE = 1 << 1,
    DESTROY_ON_OWNER_BLOCK = 1 << 2,
    
    DESTROY_ON_DAMAGE_BLOCK = DESTROY_ON_OWNER_BLOCK | DESTROY_ON_OWNER_BLOCK
}
}
