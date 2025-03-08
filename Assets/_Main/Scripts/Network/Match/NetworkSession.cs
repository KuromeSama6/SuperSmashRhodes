using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.FlatBuffers;
using NUnit.Framework;
using SuperSmashRhodes.Network.Rollbit;
using SuperSmashRhodes.Util;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

namespace SuperSmashRhodes.Network {
public class NetworkSession : IDisposable, IPacketHandler {
    public RollbitClientConfiguration config { get; private set; }
    public TcpClient client { get; private set; }
    public bool connected => client.Connected;
    public ClientStatus status { get; private set; } = ClientStatus.NOT_CONNECTED;

    public UnityEvent<bool> onConnected { get; } = new();
    public UnityEvent onEstablished { get; } = new();
    public UnityEvent onDisconnected { get; } = new();
    public UnityEvent onDisposed { get; } = new();

    public int lastPingLatency { get; private set; }
    public P2PConnector p2pConnector { get; private set; }
    public int allocatedUdpPort { get; private set; }
    
    private NetworkStream stream;
    private Timer heartbeatTimer;
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private readonly Dictionary<Type, List<RegisteredPacketHandler>> packetHandlers = new();
    private readonly Dictionary<uint, SentPacketWaitTask> requests = new();
    private uint requestIdCounter = 1;
    
    public NetworkSession(RollbitClientConfiguration config) {
        this.config = config;
        client = new TcpClient();
        client.ConnectAsync(config.host, config.port).ContinueWith(ConnectTaskCallback);
        status = ClientStatus.CONNECTING;
        client.ReceiveBufferSize = 1048576;
        client.SendBufferSize = 1048576;
    }

    private async Task SendAsync(byte[] msg) {
        if (!connected) return;
        await stream.WriteAsync(msg, 0, msg.Length);
        await stream.FlushAsync();
    }

    public async Task<uint?> SendPacket(ServerboundPacket packet) {
        try {
            if (!connected) return null;
            uint requestId = requestIdCounter++;
            var data = RollbitCodec.CreateOutboundPacket(packet, requestId, config.aesKey);
            
            await SendAsync(data);
            return requestId;
            
        } catch (Exception e) {
            Debug.LogError("Failed to send packet");
            Debug.LogException(e);
        }
        
        return null;
    }
    
    public async Task<T> SendPacket<T>(ServerboundPacket packet) where T : ClientboundPacket {
        var requestId = await SendPacket(packet);
        if (requestId == null) return null;
        
        var waitTask = new SentPacketWaitTask(packet);
        requests[requestId.Value] = waitTask;
        
        var response = await waitTask.waitHandle.Task;
        requests.Remove(requestId.Value);
        return (T)response;
    }

    public void AddPacketHandler(IPacketHandler handler) {
        // search for methods
        foreach (var method in handler.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
            var attr = method.GetCustomAttribute<PacketHandlerAttribute>();
            if (attr == null) continue;
         
            var parameters = method.GetParameters();
            if (parameters.Length != 1) {
                Debug.LogError($"Invalid packet handler method {method}, must have 1 parameter");
                continue;
            }
            
            var packetType = parameters[0];
            if (!typeof(ClientboundPacket).IsAssignableFrom(packetType.ParameterType)) {
                Debug.LogError($"Invalid packet handler method {method}, parameter must be a subclass of ClientboundPacket");
                continue;
            }

            var registeredHandler = new RegisteredPacketHandler(handler.GetType(), handler, method);
            if (!packetHandlers.ContainsKey(packetType.ParameterType)) {
                packetHandlers[packetType.ParameterType] = new List<RegisteredPacketHandler>();
            }
            
            packetHandlers[packetType.ParameterType].Add(registeredHandler);
            // Debug.Log($"Added packet handler for {packetType.ParameterType} on {method}@{handler}");
        }
    }
    
    private void ConnectTaskCallback(Task task) { 
        Debug.Log("Connected: " + connected);
        if (!connected) {
            Debug.LogError("Failed to connect to server");
            onDisconnected.Invoke();
            Dispose();
            return;
        }

        lock (this) {
            status = ClientStatus.HANDSHAKE_WAIT;
        }
        
        stream = client.GetStream();
        Task.Run(() => ReceiveLoop(cancellationTokenSource.Token));
        AddPacketHandler(this);
        
        onConnected.Invoke(connected);
        
        // handshake
        SendPacket<PacketPlayOutHandshake>(new PacketPlayInHandshake(this, PlayerRole.PLAYER)).ContinueWith(packet => {
            HandleHandshakeResponse(packet.Result);
        });
        
    }

    private void HandleHandshakeResponse(PacketPlayOutHandshake packet) {
        Debug.Log("Handshake response: " + packet);
        heartbeatTimer?.Dispose();
        heartbeatTimer = new Timer(HeartbeatTick, this, 0, packet.heartbeatInterval / 2);
        onEstablished.Invoke();
        lock (this) {
            status = ClientStatus.ESTABLISHED;
        }
            
        // create p2p connector
        try {
            allocatedUdpPort = NetworkUtil.AllocateFreePort();
            p2pConnector = new(this, packet.udpToken, packet.udpAddressString, packet.udpPort, allocatedUdpPort);
            p2pConnector.SendEndpointRegistrationPacket();
            
        } catch (Exception e) {
            Debug.LogError("Failed to create P2P connector");
            Debug.LogException(e);
        }
    }
    
    private async void ReceiveLoop(CancellationToken token) {
        var headerBuf = new byte[32];
        while (!cancellationTokenSource.Token.IsCancellationRequested) {
            if (!client.Connected) {
                Debug.Log("Client connection closed");
                break;
            }

            try {
                int headerSize = await stream.ReadAsync(headerBuf, 0, headerBuf.Length, token);
                if (headerSize == 0) {
                    Debug.Log("Server connection closed");
                    break;
                }
                
                if (headerSize != 32) {
                    Debug.LogError("Invalid header size, disconnected");
                    Disconnect(ClientDisconnectionReason.PROTOCOL_ERROR, "invalid header size received");
                    break;
                }
                
                // decrypt header
                if (config.aesKey?.Length > 0) {
                    RollbitCodec.Decrypt(config.aesKey, headerBuf);
                }
                
                // bytes 6~9 uint length
                uint packetLength = BinaryPrimitives.ReadUInt32BigEndian(headerBuf[6..10]);
                var padding = (16 - packetLength % 16) % 16;
                packetLength += padding;

                if (packetLength < 32) {
                    Debug.LogError("Packet length is invalid, disconnected");
                    Disconnect(ClientDisconnectionReason.PROTOCOL_ERROR, "invalid packet length");
                    break;
                }
                
                var bodyBuf = new byte[packetLength - 32];
                _ = await stream.ReadAsync(bodyBuf, 0, bodyBuf.Length, token);
                // decrypt body
                if (config.aesKey?.Length > 0) {
                    RollbitCodec.Decrypt(config.aesKey, bodyBuf);
                }
                
                ByteBuf buf = new(packetLength);
                buf.SetBytes(0, headerBuf);
                buf.SetBytes(32, bodyBuf);
                
                // create header
                // Debug.Log($"Header: {headerBuf.Format()}");

                var header = new PacketHeader(buf);
                if (header.magic != PacketHeader.ROLLBIT_MAGIC) {
                    throw new ArgumentException($"Invalid magic number: {header.magic:X}");
                }
                
                // create packet
                var packetType = header.type.GetClass();
                if (packetType == null) {
                    Debug.LogError($"Unknown packet type: {header.type}");
                    Disconnect(ClientDisconnectionReason.PROTOCOL_ERROR, $"unknown packet type {header.type}");
                    continue;
                }
                
                var packet = (ClientboundPacket)Activator.CreateInstance(packetType, this, header, new ByteBuf(bodyBuf));
                // Debug.Log($"Received packet: {packet}");
                // Debug.Log($"body: {bodyBuf.Format()}");
                // Debug.Log(packet.header);
                
                // find wait task
                if (requests.TryGetValue(packet.header.requestId, out var waitTask)) {
                    waitTask.waitHandle.SetResult(packet);
                }
                
                // find handler
                if (!packetHandlers.ContainsKey(packetType)) {
                    if (waitTask == null) {
                        Debug.LogError($"No handlers for packet type {packetType}");
                        continue;
                    }
                    
                } else { 
                    foreach (var handler in packetHandlers[packetType]) {
                        handler.Invoke(packet);
                    }
                }

            } catch (Exception e) {
                Debug.LogError($"Connection closed due to exception:");
                Debug.LogException(e);
                Disconnect(ClientDisconnectionReason.CLIENT_ERROR, "client exception");
                break;
            }
        } 
        
        Debug.Log("Receive loop ended");
        onDisconnected.Invoke();
        client.Close();
    }

    private async void HeartbeatTick(object state) {
        var stopwatch = Stopwatch.StartNew();
        await SendPacket<PacketPlayOutGenericResponse>(new PacketPlayInHeartbeat(this));
        lastPingLatency = (int)stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();
    }
    
    public void Disconnect(ClientDisconnectionReason reason, string remarks = "") {
        SendPacket(new PacketPlayInDisconnect(this, reason, remarks));
    }

    public void StopP2PConnector() {
        p2pConnector?.Dispose();
        p2pConnector = null;
    }
    
    public void Dispose() {
        client.Dispose();
        stream.Dispose();
        p2pConnector?.Dispose();
        cancellationTokenSource.Dispose();
        heartbeatTimer?.Dispose();
        
        onConnected.RemoveAllListeners();
        onDisconnected.RemoveAllListeners();
        
        onDisposed.Invoke();
        onDisposed.RemoveAllListeners();
        
        Debug.Log($"NetworkSession disposed");
    }
    
    //region Packet Handlers
    [PacketHandler]
    private void OnServerDisconnect(PacketPlayOutDisconnect packet) {
        Debug.Log($"Server disconnected with {packet.code}: remarks: {packet.remarks};");
    }

    [PacketHandler]
    private void OnGenericResponse(PacketPlayOutGenericResponse packet) { }
    //endregion
}

public class SentPacketWaitTask {
    public readonly ServerboundPacket packet;
    public readonly TaskCompletionSource<ClientboundPacket> waitHandle;
    
    public SentPacketWaitTask(ServerboundPacket packet) {
        this.packet = packet;
        waitHandle = new();
    }
}
}
