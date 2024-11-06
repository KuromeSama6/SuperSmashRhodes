using SuperSmashRhodes.Battle.Enums;

namespace SuperSmashRhodes.Util {
public static class PhysicsUtil {
    public static float NormalizeRelativeDirecionalForce(float force, EntityFacing facing) {
        return force * (facing == EntityFacing.LEFT ? 1 : -1);
    }
}
}
