using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Google.FlatBuffers;
using Spine;
using Unity.Collections;

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

    public static int AllocateFreePort() {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    public static int CalcFletcher32(NativeArray<byte> data) {
        uint sum1 = 0;
        uint sum2 = 0;

        int index;
        for (index = 0; index < data.Length; ++index) {
            sum1 = (sum1 + data[index]) % 0xffff;
            sum2 = (sum2 + sum1) % 0xffff;
        }
        return unchecked((int)((sum2 << 16) | sum1));
    }

    public static string GetLocalIPAddress() {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList) {
            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
}
