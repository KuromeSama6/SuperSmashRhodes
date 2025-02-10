using System;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public interface IEntityBoundingBox {
    public BoundingBoxType type { get; }
    public Collider2D box { get; }
    public PlayerCharacter owningPlayer { get; }
}


[Flags]
public enum BoundingBoxType {
    PUSHBOX = 1 << 0,
    HITBOX = 1 << 1,
    HURTBOX = 1 << 2,
    
    CHR_MAIN_PUSHBOX = PUSHBOX | HURTBOX
}

}
