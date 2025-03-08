using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SuperSmashRhodes.Util {
public struct StunResult {
    public IPAddress publicIP { get; set; }
    public int publicPort { get; set; }

    public override string ToString() {
        return $"{publicIP}:{publicPort}";
    }
}

public class StunClient {
    public static async Task<StunResult> GetPublicEndpointAsync(int clientPort = 0, string stunServer = "stun.l.google.com", int stunPort = 19302) {
        using (UdpClient udpClient = new UdpClient(clientPort)) {
            try {
                udpClient.Connect(stunServer, stunPort);

                // Construct a STUN Binding Request (20 bytes)
                byte[] stunRequest = new byte[20];
                stunRequest[0] = 0x00; // Binding Request
                stunRequest[1] = 0x01;
                stunRequest[2] = 0x00; // Message Length (0 for basic request)
                stunRequest[3] = 0x00;

                // Generate random Transaction ID (12 bytes)
                Random random = new Random();
                byte[] transactionId = new byte[12];
                random.NextBytes(transactionId);
                Array.Copy(transactionId, 0, stunRequest, 8, 12);

                // Send STUN request
                await udpClient.SendAsync(stunRequest, stunRequest.Length);

                // Receive response
                UdpReceiveResult response = await udpClient.ReceiveAsync();
                byte[] stunResponse = response.Buffer;

                // Validate STUN response (should be at least 20 bytes and of type Binding Response)
                if (stunResponse.Length >= 32 && stunResponse[0] == 0x01 && stunResponse[1] == 0x01) {
                    int port = (stunResponse[26] << 8) | stunResponse[27];
                    port ^= 0x2112; // XOR with STUN magic cookie

                    byte[] ipBytes = {
                        (byte)(stunResponse[28] ^ 0x21), (byte)(stunResponse[29] ^ 0x12), (byte)(stunResponse[30] ^ 0xA4), (byte)(stunResponse[31] ^ 0x42)
                    };
                    IPAddress publicIP = new IPAddress(ipBytes);

                    return new StunResult {
                        publicIP = publicIP,
                        publicPort = port
                    };
                }
            } catch (Exception ex) {
                Console.WriteLine($"STUN request failed: {ex.Message}");
            }
        }

        return new StunResult {
            publicIP = IPAddress.None,
            publicPort = 0
        };
    }
}

}
