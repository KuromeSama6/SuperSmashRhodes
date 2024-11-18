using SuperSmashRhodes.Battle.Enums;
using UnityEngine;

namespace SuperSmashRhodes.Util {
public static class PhysicsUtil {
    public static float NormalizeSide(float force, EntitySide side) {
        return force * (side == EntitySide.LEFT ? 1 : -1);
    }

    public static Vector3 NormalizeSide(Vector3 vec, EntitySide side) {
        vec.x *= side == EntitySide.LEFT ? 1 : -1;
        return vec;
    }
    
}
}
