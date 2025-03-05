using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using SuperSmashRhodes.Config.Global;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.Events;

namespace SuperSmashRhodes.Network.Rollbit.P2P {
/// <summary>
/// A class that manages the p2p connection between two clients. P2P connector abstracts the sending and reciving of P2P packets between two peers. Even when a P2P connection could not be established and packets are being relayed through the match server, P2P packets should still be sent through the P2P connector, which will handle the relaying of packets through the network session.
/// </summary>
[Obsolete]
internal class P2PConnector : IDisposable {
    public bool valid { get; private set; }
    public NetworkSession networkSession { get; private set; }
    public PacketPlayOutBeginP2P negotiationPacket { get; private set; }
    public GGPOConnectionStatus status { get; private set; }

    public int listenerPort => netManager.LocalPort;
    public bool bound => netManager != null && netManager.IsRunning;
    public bool useP2P => negotiationPacket != null && negotiationPacket.useP2P;
    public bool negotiationComplete => status == GGPOConnectionStatus.SKIPPED || status == GGPOConnectionStatus.ESTABLISHED || status == GGPOConnectionStatus.DISCONNECTED;
    public bool peerConnected => netManager != null && netManager.IsRunning && peer != null && peer.ConnectionState == ConnectionState.Connected;
    public NetPeer peer { get; private set; }
    public UnityEvent<NetworkInputFrame> onInputFrameReceived { get; } = new();
    
    private NetManager netManager;
    private EventBasedNetListener listener;
    private CancellationTokenSource cts;
    private Thread listenerThread;
    private EventBasedNatPunchListener natPunchListener;
    private int attemptsRemaining;
    
    public P2PConnector(NetworkSession networkSession) {
        this.networkSession = networkSession;
        valid = true;
        
        networkSession.onDisconnected.AddListener(OnMatchServerDisconnected);
        networkSession.onDisposed.AddListener(Dispose);
        
    }

    public void Bind() {
        listener = new();
        listener.PeerConnectedEvent += OnPeerConnected;
        listener.PeerDisconnectedEvent += OnPeerDisconnected;
        listener.ConnectionRequestEvent += OnConnectionRequest;
        listener.NetworkReceiveEvent += OnNetworkReceive;
        attemptsRemaining = Math.Max(1, networkSession.config.p2PNegotiationAttempts);
        
        netManager = new NetManager(listener);
        netManager.NatPunchEnabled = true;
        var config = networkSession.config;
        netManager.SimulateLatency = config.debugP2PLatency;
        netManager.SimulationMinLatency = config.debugP2PLatencyRange.x;
        netManager.SimulationMaxLatency = config.debugP2PLatencyRange.y;

        natPunchListener = new();
        netManager.NatPunchModule.Init(natPunchListener);
        natPunchListener.NatIntroductionRequest += OnNatIntroductionRequest;
        natPunchListener.NatIntroductionSuccess += OnNatIntroductionSuccess;
        
        netManager.Start();
        cts = new();
        
        listenerThread = new Thread(ListenerThread);
        listenerThread.Start();
        
        Debug.Log($"P2P connector bound to port {listenerPort}");
    }

    public void BeginP2P(PacketPlayOutBeginP2P packet) {
        negotiationPacket = packet;
        status = GGPOConnectionStatus.CONNECTING;
        --attemptsRemaining;
        
        Debug.Log($"P2P Connection Attempt ({attemptsRemaining} attempts left): {packet}");

        if (!useP2P) {
            Debug.LogWarning("Not using P2P. Skipping the P2P connection.");
            networkSession.SendPacket(new PacketPlayInConfirmP2P(networkSession, 0, 0));
            status = GGPOConnectionStatus.SKIPPED;
            return;
        }
        
        Debug.Log("sending nat introduce request");
        netManager.NatPunchModule.SendNatIntroduceRequest(packet.peerAddressString, packet.peerPort, packet.verifier.ToString());
        
        Debug.Log($"connecting to: {packet.peerAddressString}:{packet.peerPort}");
        netManager.Connect(packet.peerAddressString, packet.peerPort, packet.verifier.ToString());
    }

    // public void SendPacket(NetPeer peer, P2PPacket packet) {
    //     var data = RollbitCodec.CreateOutboundPacket(packet.header, packet.Serialize(), 0, negotiationPacket.aesKey);
    //     peer.Send(data, DeliveryMethod.ReliableOrdered);
    // }
    //
    // public void SendInput(int frame, InputFrame[] frames) {
    //     if (status == GGPOConnectionStatus.ESTABLISHED) {
    //         SendPacket(peer, new PacketPlayP2PInput(this, frame, frames));
    //         
    //     } else if (status == GGPOConnectionStatus.SKIPPED) {
    //         //TODO: relay input
    //         
    //     } else {
    //         throw new InvalidOperationException("Cannot send input before P2P connection is established.");
    //     }
    // }
    
    private void ListenerThread() { 
        while (!cts.IsCancellationRequested) {
            try {
                netManager.NatPunchModule.PollEvents();
                netManager.PollEvents();
                // Thread.Sleep(15);

            } 
            catch (ThreadAbortException) { break;} 
            catch (ThreadInterruptedException) { break;} 
            catch (Exception e) {
                Debug.Log("Error in P2P Connector listener thread:");
                Debug.LogError(e); 
            }       
        }
    }

    private void OnNatIntroductionRequest(IPEndPoint local, IPEndPoint remote, string token) {
        Debug.Log($"intro request: {local}, {remote}, {token}");
    }
    
    private void OnNatIntroductionSuccess(IPEndPoint targetEndPoint, NatAddressType type, string token) {
        Debug.Log($"Introduce success: {targetEndPoint}, {type}, {token}");
    }
    
    
    private void OnConnectionRequest(ConnectionRequest request) {
        if (peer != null) {
            Debug.LogError("Peer already connected.");
            request.Reject();
            return;
        } 
        
        Debug.Log($"Connection request: {request}, key/verifier: {request.Data.GetString()}, expected: {negotiationPacket.expectedVerifier}");
        request.Accept();
    }
    
    private void OnPeerConnected(NetPeer peer) {
        this.peer = peer;
        Debug.Log($"peer connected {peer.RemoteId}. Waiting for confirmation.");
        
        // SendPacket(peer, new PacketPlayP2PHandshake(this, negotiationPacket));
    }
    
    private void OnPeerDisconnected(NetPeer peer, DisconnectInfo info) {
        if (status == GGPOConnectionStatus.CONNECTING) {
            Debug.LogWarning($"peer disconnected: {info.Reason}. Data: {info.AdditionalData} Error: {info.SocketErrorCode}");
            if (attemptsRemaining > 0) {
                Debug.Log("Retrying P2P connection.");
                BeginP2P(negotiationPacket);
            } else {
                Debug.LogWarning("P2P connection failed. Fallback to relay.");
                FallbackToRelay();
            }
        }
    }
    
    private void OnNetworkReceive(NetPeer netPeer, NetPacketReader reader, byte channel, DeliveryMethod deliverymethod) {
        var bytes = reader.GetRemainingBytes();
        RollbitCodec.Decrypt(negotiationPacket.aesKey, bytes);

        var buf = new ByteBuf(bytes);
        PacketHeader header = new(buf);

        if (header.magic != PacketHeader.P2P_MAGIC) {
            Debug.LogError("Received packet with invalid magic. Switching to relay.");
            FallbackToRelay();
            return;
        }
        
        if (header.version != ApplicationGlobalSettings.inst.rollbitVersion) {
            Debug.LogError($"Received packet with invalid version. Game is on {ApplicationGlobalSettings.inst.rollbitVersion}, received {header.version}. Switching to relay.");
            FallbackToRelay();
            return;
        }
        
        var body = buf.Slice(32);
        var type = header.type.GetClass();
        
        // var packet = (P2PPacket)Activator.CreateInstance(type, this, header, body);
        // HandlePacket(packet);
    }

    // private void HandlePacket(P2PPacket packet) {
    //     if (status != GGPOConnectionStatus.CONNECTING && status != GGPOConnectionStatus.ESTABLISHED) {
    //         return;
    //     }
    //     
    //     if (packet is PacketPlayP2PHandshake handshake && status == GGPOConnectionStatus.CONNECTING) {
    //         bool verified = handshake.verifier == negotiationPacket.expectedVerifier;
    //         if (!verified) {
    //             Debug.LogWarning($"Received handshake with invalid verifier: {handshake.verifier}. Expected: {negotiationPacket.expectedVerifier}");
    //         }
    //         
    //         Debug.Log($"Peer handshake success. Heartbeat interval: {handshake.heartbeatInterval}"); 
    //         _ = networkSession.SendPacket<PacketPlayOutGenericResponse>(new PacketPlayInConfirmP2P(networkSession, verified ? 1 : 2, handshake.nonce));
    //         status = GGPOConnectionStatus.ESTABLISHED;
    //
    //     } else {
    //         if (status != GGPOConnectionStatus.ESTABLISHED) {
    //             Debug.LogWarning("received packet before handshake was establishe1d!");
    //             // FallbackToRelay();
    //             return;
    //         }
    //     }
    //
    //     if (packet is PacketPlayP2PInput input) {
    //         lock (onInputFrameReceived) {
    //             onInputFrameReceived.Invoke(input.ToInputFrame());
    //         }
    //     }
    //     
    // }
    
    private void OnMatchServerDisconnected() {
        Dispose();
    }

    private void DisconnectOrFallback(string remarks = "") {
        if (status == GGPOConnectionStatus.ESTABLISHED) {
            StopUDPServer();
            networkSession.Disconnect(ClientDisconnectionReason.P2P_ERROR, remarks);

        } else if (status == GGPOConnectionStatus.CONNECTING) {
            FallbackToRelay();
        }
    }
    
    private void FallbackToRelay() {
        networkSession.SendPacket(new PacketPlayInConfirmP2P(networkSession, 0, 0));
        status = GGPOConnectionStatus.SKIPPED;
        StopUDPServer();
    }
    
    public void StopUDPServer() {
        listenerThread.Abort();
        netManager.DisconnectAll();
        netManager.Stop();
        netManager = null;
        peer = null;
        cts.Cancel(); 
        cts.Dispose();
    }
    
    public void Dispose() {
        StopUDPServer();
        valid = false;
        Debug.Log("P2P connector disposed.");
    }
}
}
