namespace SuperSmashRhodes.Util {
public static class BitUtil {
    public static bool AllBitsSet(int flag) {
        return (flag & (flag + 1)) == 0;
    }

    public static bool CheckFlag(int value, int flag) {
        return value == (value | flag);
    }
}
}
