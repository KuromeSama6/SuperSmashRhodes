using System;

namespace SuperSmashRhodes.Util {
public static class BitUtil {
    public static bool AllBitsSet(ulong flag) {
        return (flag & (flag + 1)) == 0;
    }

    public static bool CheckFlag(ulong value, ulong flag) {
        // return value == (value | flag);
        return (value & flag) != 0;
    }
    
    public static bool CheckFlag(this Enum value, ulong flag) {
        return CheckFlag(Convert.ToUInt64(value), flag);
    }
}
}
