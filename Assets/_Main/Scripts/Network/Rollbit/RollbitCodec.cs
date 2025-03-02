using System.Security.Cryptography;
using System.Text;
using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public static class RollbitCodec {
    public static byte[] CreateOutboundPacket(ServerboundPacket packet, uint requestId = 0, string aesKey = null) {
        var header = packet.header;
        var body = packet.Serialize();
        var padding = (16 - body.size % 16) % 16;
            
        var buf = new ByteBuf(32 + body.size + padding);
        header.requestId = requestId;
        
        buf.SetBytes(0, header.bytes);
        buf.SetBytes(32, body.bytes);
            
        // Debug.Log(buf.Format());
            
        var bytes = buf.bytes;
        if (aesKey != null && aesKey.Length > 0) {
            Encrypt(aesKey, bytes);
        }

        return bytes;
    }
    
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
