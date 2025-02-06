using System.Text;
using Google.FlatBuffers;
using Spine;

namespace SuperSmashRhodes.Util {
public static class NetworkUtil {
    public static string Format(this byte[] bytes, int length = -1) {
        StringBuilder sb = new();
        for (int i = 0; i < (length == -1 ? bytes.Length : length); i++) {
            sb.Append($"{bytes[i]:X2}");
            sb.Append(" ");
        }
        
        return sb.ToString(); 
    }

    public static string Format(this ByteBuf buf) {
        return Format(buf.bytes);
    }
}
}
