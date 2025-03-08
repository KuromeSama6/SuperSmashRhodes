using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SuperSmashRhodes.Network.Rollbit;
using SuperSmashRhodes.Util;
using Unity.VisualScripting;
using UnityEngine;
using Timer = System.Threading.Timer;

namespace SuperSmashRhodes.Network {
public class P2PConnector : IDisposable {
    public int udpPort { get; private set; }
    public bool valid { get; private set; }
    public string token { get; private set; }
    public IPEndPoint peerEndpoint { get; private set; }
    
    private readonly UdpClient client;
    private readonly CancellationTokenSource cts;
    private readonly NetworkSession session;
    private readonly IPEndPoint serverEndpoint;
    private readonly Thread receiveThread;
    private readonly Timer keepOpenTimer;
    private Timer holePunchTimer;
    
    public P2PConnector(NetworkSession session, string token, string address, int port, int localPort) {
        this.session = session;
        this.token = token;
        valid = true;

        udpPort = localPort;
        client = new UdpClient(udpPort);
        
        serverEndpoint = new IPEndPoint(IPAddress.Parse(address), port);
        
        Debug.Log($"P2PConnector: Connection to {serverEndpoint} from local port {udpPort} with token {token}");

        cts = new();
        receiveThread = new Thread(ReceiveLoop);
        receiveThread.IsBackground = true;
        receiveThread.Start();

        keepOpenTimer = new(SendEndpointRegistrationPacket, this, 2000, 2000);
    }
 
    private void ReceiveLoop() {
        while (!cts.IsCancellationRequested) {
            try {
                if (client.Available > 0) {
                    IPEndPoint from = null;
                    var result = client.Receive(ref from);
                    var buf = new ByteBuf(result);

                    // Debug.Log($"Received {buf}");
                }

                Thread.Sleep(10);

            } catch (ThreadAbortException e) {
                break;
            }
            catch (Exception e) {
                Debug.LogError("Exception in P2P Connector received thread:");
                Debug.LogException(e);
            }
        }
        
        Dispose();
    }

    public void Send(ServerboundPacket packet) {
        var header = packet.CreateHeader(token);
        var body = packet.Serialize();

        var data = RollbitCodec.CreateOutboundPacket(header, body, 0u, session.config.aesKey);
        client.SendAsync(data, data.Length, serverEndpoint);
        
        // Debug.Log($"send to {remoteEndPoint} : {data.ToHexString()}");
    }

    public void SendEndpointRegistrationPacket(object _ = null) {
        var packet = new PacketPlayInUDPRegisterEndpoint(session);
        Send(packet);
    }
    
    public void BeginHolePunch(string address, int port) {
        peerEndpoint = new IPEndPoint(IPAddress.Parse(address), port);

        holePunchTimer = new(_ => {
            var data = new byte[] {0x0d, 0x00, 0x07, 0x21 };
            client.Send(data, data.Length, peerEndpoint);
            Debug.Log("try hole punch");
        }, this, 500, 500);

    }
    
    public void Dispose() {
        valid = false;

        if (receiveThread.IsAlive) {
            receiveThread.Abort();
        }
        
        cts.Cancel();
        client.Close();
        client.Dispose();
        keepOpenTimer?.Dispose();
        holePunchTimer?.Dispose();
        holePunchTimer = null;
        Debug.Log("P2PConnector disposed");
    }

    public static async Task<ByteBuf> SendSingle(byte[] data, int localPort, string address, int port) {
        using var client = new UdpClient(localPort);
        client.Client.SendTimeout = 5000;
        client.Client.ReceiveTimeout = 1000;
        var remoteEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
        await client.SendAsync(data, data.Length, remoteEndPoint);
        
        var result = await client.ReceiveAsync();
        return new ByteBuf(result.Buffer);
    }
}
}
