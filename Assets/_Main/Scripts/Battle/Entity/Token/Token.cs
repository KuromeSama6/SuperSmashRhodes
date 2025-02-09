using System;
using SuperSmashRhodes.Battle.State;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
/**
 * A token is a stationary entity summoned by a character. Different from projectiles, tokens cannot be interacted with by players (and by extension their hitboxes). Torappu uses the phrase "tokens" to refer to these entities, so the term is used here as well.
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
