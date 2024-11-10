using SuperSmashRhodes.Battle.Enums;

namespace SuperSmashRhodes.Util {
public static class PhysicsUtil {
    public static float NormalizeRelativeDirecionalForce(float force, EntitySide side) {
        return force * (side == EntitySide.LEFT ? 1 : -1);
    }
}
}
