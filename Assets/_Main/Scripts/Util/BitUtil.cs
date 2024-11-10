namespace SuperSmashRhodes.Util {
public static class BitUtil {
    public static bool AllBitsSet(ulong flag) {
        return (flag & (flag + 1)) == 0;
    }

    public static bool CheckFlag(ulong value, ulong flag) {
        return value == (value | flag);
    }
}
}
