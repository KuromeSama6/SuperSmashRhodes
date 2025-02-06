using System.Security.Cryptography;
using System.Text;

namespace SuperSmashRhodes.Network.Rollbit {
public static class PacketCodec {
    public static void Decrypt(string key, byte[] data) {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        using (Aes aes = Aes.Create()) {
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
            aes.Key = keyBytes;

            using (ICryptoTransform decryptor = aes.CreateDecryptor())
            {
                decryptor.TransformBlock(data, 0, data.Length, data, 0);
            }
        }
    }

    public static void Encrypt(string key, byte[] data) {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        using (Aes aes = Aes.Create()) {
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
            aes.Key = keyBytes;

            using (ICryptoTransform encryptor = aes.CreateEncryptor()) {
                encryptor.TransformBlock(data, 0, data.Length, data, 0);
            }
        }
    }
}
}
