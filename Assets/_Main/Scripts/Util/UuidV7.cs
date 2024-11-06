using System;

namespace SuperSmashRhodes.Util {
public static class UuidV7 {
    public static Guid Generate() {
        // Get the current timestamp in milliseconds since Unix epoch
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Create a byte array to hold the UUID
        byte[] uuidBytes = new byte[16];

        // Fill the first 6 bytes with the timestamp (48 bits)
        uuidBytes[0] = (byte)((timestamp >> 40) & 0xFF);
        uuidBytes[1] = (byte)((timestamp >> 32) & 0xFF);
        uuidBytes[2] = (byte)((timestamp >> 24) & 0xFF);
        uuidBytes[3] = (byte)((timestamp >> 16) & 0xFF);
        uuidBytes[4] = (byte)((timestamp >> 8) & 0xFF);
        uuidBytes[5] = (byte)(timestamp & 0xFF);

        // Set the version to 7 (0111 in binary)
        uuidBytes[6] = (byte)((7 << 4) | (uuidBytes[6] & 0x0F));

        // Generate 10 random bytes for the remaining 74 bits
        Random random = new Random();
        random.NextBytes(uuidBytes.AsSpan(6, 10));

        // Set the variant bits (10xxxxxx)
        uuidBytes[8] = (byte)((uuidBytes[8] & 0x3F) | 0x80);

        return new Guid(uuidBytes);
    }
}
}
