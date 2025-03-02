using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace SuperSmashRhodes.Network.Rollbit.P2P {
/// <summary>
/// A class that manages the p2p connection between two clients. P2P connector abstracts the sending and reciving of P2P packets between two peers. Even when a P2P connection could not be established and packets are being relayed through the match server, P2P packets should still be sent through the P2P connector, which will handle the relaying of packets through the network session.
/// </summary>
public class P2PConnector : IDisposable {
    public bool valid { get; private set; }
    public NetworkSession networkSession { get; private set; }
    public PacketPlayOutBeginP2P beginP2PPacket { get; private set; }
    public P2PNegotiationStatus status { get; private set; }

    public int listenerPort => netManager.LocalPort;
    public bool bound => netManager != null && netManager.IsRunning;
    public bool useP2P => beginP2PPacket != null && beginP2PPacket.useP2P;
    public bool negotiationComplete => status == P2PNegotiationStatus.SKIPPED || status == P2PNegotiationStatus.ESTABLISHED || status == P2PNegotiationStatus.FAILED;

    private NetManager netManager;
    private EventBasedNetListener listener;
    private CancellationTokenSource cts;
    private Thread listenerThread;
    private EventBasedNatPunchListener natPunchListener;
    private NetPeer peer;
    
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
        
        netManager = new NetManager(listener);
        netManager.NatPunchEnabled = true;

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
        beginP2PPacket = packet;
        status = P2PNegotiationStatus.NEGOTIATING;
        
        Debug.Log($"Begin p2p: {packet}");

        if (!useP2P) {
            Debug.LogWarning("Not using P2P. Skipping the P2P connection.");
            networkSession.SendPacket(new PacketPlayInConfirmP2P(networkSession, 0, 0));
            status = P2PNegotiationStatus.SKIPPED;
            return;
        }
        
        Debug.Log("sending nat introduce request");
        netManager.NatPunchModule.SendNatIntroduceRequest(packet.peerAddressString, packet.peerPort, packet.verifier.ToString());
        
        Debug.Log($"connecting to: {packet.peerAddressString}:{packet.peerPort}");
        netManager.Connect(packet.peerAddressString, packet.peerPort, packet.verifier.ToString());
    }
    
    private void ListenerThread() { 
        while (!cts.IsCancellationRequested) {
            try {
                netManager.NatPunchModule.PollEvents();
                netManager.PollEvents();
                Thread.Sleep(15);

            } 
            catch (ThreadAbortException) { break;} 
            catch (ThreadInterruptedException) { break;} 
            catch (Exception e) {
                Debug.Log("Error in P2P Connector listener thread: ");
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
        
        Debug.Log($"Connection request: {request}, key/verifier: {request.Data.GetString()}, expected: {beginP2PPacket.expectedVerifier}");
        request.Accept();
    }
    
    private void OnPeerConnected(NetPeer peer) {
        this.peer = peer;
        Debug.Log($"peer connected {peer.RemoteId}. Waiting for confirmation.");
    }
    
    private void OnPeerDisconnected(NetPeer peer, DisconnectInfo info) {
        if (status == P2PNegotiationStatus.NEGOTIATING) {
            Debug.LogWarning($"peer disconnected: {info.Reason}. Data: {info.AdditionalData} Error: {info.SocketErrorCode}. Switching to relay.");
            status = P2PNegotiationStatus.SKIPPED;
            networkSession.SendPacket(new PacketPlayInConfirmP2P(networkSession, 0, 0));

            StopUDPServer();
        }
    }
    
    private void OnNetworkReceive(NetPeer netPeer, NetPacketReader reader, byte channel, DeliveryMethod deliverymethod) {
        Debug.Log($"received packet from {netPeer.RemoteId}");
    }
    
    private void OnMatchServerDisconnected() {
        Dispose();
    }

    public void StopUDPServer() {
        listenerThread.Abort();
        
        netManager.DisconnectAll();
        netManager.Stop();
        netManager = null;
        
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
