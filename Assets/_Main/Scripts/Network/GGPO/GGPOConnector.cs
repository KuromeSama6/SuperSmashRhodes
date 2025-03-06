using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.GGPOWrapper.Packet;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Match.Player;
using SuperSmashRhodes.Network;
using SuperSmashRhodes.Network.Rollbit;
using SuperSmashRhodes.Network.Rollbit.P2P;
using SuperSmashRhodes.Network.RoomManagement;
using SuperSmashRhodes.Util;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityGGPO;

namespace SuperSmashRhodes.GGPOWrapper {
/// <summary>
/// Wrapper class for interacting with the GGPO sdk.
/// </summary>
public class GGPOConnector : IDisposable {
    public static readonly int INPUT_BUFFER_SIZE = 128;
    
    public static GGPOConnector inst { get; private set; }
    public GGPOConnectionStatus status { get; private set; } = GGPOConnectionStatus.STANDBY;
    public int port { get; private set; }
    public int framesAhead { get; private set; }
    public Dictionary<int, GGPOPlayer> players { get; } = new();
    public UnityEvent onDisconnected { get; } = new();
    
    private readonly NetworkRoom room;
    private PacketPlayOutBeginP2P negotiationPacket;
    private readonly Dictionary<int, int> playerIdToHandleMap = new();
    private readonly IConnectorCallbackHandler callbackHandler;
    private readonly List<ChannelSubpacket> packetQueue = new();
    private readonly HashSet<int> auxiliaryInputsReceived = new();
    private int gamestateHandleCounter = 0;

    public bool connected => status == GGPOConnectionStatus.ESTABLISHED || status == GGPOConnectionStatus.SYNCHRONIZING;
    
    public GGPOConnector(NetworkRoom room, IConnectorCallbackHandler callbackHandler) {
        if (inst != null) {
            throw new InvalidOperationException("The last GGPOConnector instance has not been disposed yet. It must be disposed before creating a new one.");
        }

        this.room = room;
        inst = this;
        this.callbackHandler = callbackHandler;
    }

    public bool Bind() {
        if (status != GGPOConnectionStatus.STANDBY) {
            throw new InvalidOperationException("The GGPOConnector is already bound.");
        }

        port = room.localPlayer.playerId == 0 ? room.session.config.ggpoPortP1 : room.session.config.ggpoPortP2;

        if (GGPO.Session.IsStarted()) {
            GGPO.Session.CloseSession();   
        }
        
        GGPOStatusCode res = (GGPOStatusCode)GGPO.Session.StartSessionCustomInputSize(OnGameBegin, OnAdvanceFrame, OnLoadGameState, OnLogGameState, OnSaveGameState, OnFreeBuffer, OnEventConnectedToPeer, OnEventSynchronizingWithPeer, OnEventSynchronizedWithPeer, OnEventRunning, OnEventConnectionInterrupted, OnEventConnectionResumed, OnEventDisconnectedFromPeer, OnEventEventcodeTimesync, "ssr_networkgame", 2, port, INPUT_BUFFER_SIZE);

        GGPO.Session.SetDisconnectTimeout(3000);
        GGPO.Session.SetDisconnectNotifyStart(1500);
        GGPO.Session.Init(OnLog);
        
        if (res != 0) {
            Debug.LogError($"Failed to start GGPO session: {res}");
            return false;
        }

        Debug.Log($"GGPO session started on port {port}");
        return true;
    }

    public void BeginP2P(PacketPlayOutBeginP2P packet) {
        negotiationPacket = packet;
        status = GGPOConnectionStatus.CONNECTING;
        
        Debug.Log($"Starting P2P connection: {packet}");

        // local player 
        {
            var success = AddPlayer(room.localPlayer, new() {
                type = GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL,
                player_num = room.localPlayer.playerId + 1,
                ip_address = "",
                port = 0
            });
            if (!success) {
                throw new Exception("Failed to add local player");
            }
        }
        
        // remote player
        {
            var success = AddPlayer(room.remotePlayer, new() {
                type = GGPOPlayerType.GGPO_PLAYERTYPE_REMOTE,
                player_num = room.remotePlayer.playerId + 1,
                ip_address = packet.peerAddressString,
                port = (ushort)packet.peerPort,
            });
            if (!success) {
                throw new Exception("Failed to add remote player");
            }
        }

    }

    private bool AddPlayer(Player ssrPlayer, GGPOPlayer player) {
        var ret = (GGPOStatusCode)GGPO.Session.AddPlayer(player, out var handle);
        if (ret != GGPOStatusCode.OK) {
            Debug.LogError($"Could not add player: {ret}");
            return false;
        }
        
        players[handle] = player;
        playerIdToHandleMap[ssrPlayer.playerId] = handle;
        Debug.Log($"add player {ssrPlayer.playerId} handle {handle}");
        return true;
    }

    public void QueueInputChannelPacket(ChannelSubpacket packet) {
        packetQueue.Add(packet);
    }
    
    public GGPOStatusCode SendQueuedInputPacketsSync(int playerId) {
        if (!playerIdToHandleMap.ContainsKey(playerId)) return GGPOStatusCode.OK;
        var packet = new InputChannelPacket(playerId);
        packet.subpackets.AddRange(packetQueue);
        packetQueue.Clear();
        
        var data = packet.Serialize();
        // Debug.Log($"send {playerId}: {data.ToHexString()}");
        return (GGPOStatusCode)GGPO.Session.AddLocalInput(playerIdToHandleMap[playerId], INPUT_BUFFER_SIZE, data);
    }

    public InputChannelPacket[] ReadInputChannelSync(out GGPOStatusCode result) {
        var data = GGPO.Session.SynchronizeInput(2, INPUT_BUFFER_SIZE, out int res, out int disconnectFlags);
        result = (GGPOStatusCode)res;
        
        if (result != GGPOStatusCode.OK) {
            return null;
        }
        
        var ret = new InputChannelPacket[2];
        for (int i = 0; i < 2; i++) {
            if (data[i][0] != InputChannelPacket.MAGIC) {
                ret[i] = null;
                continue;
            }
            
            var packet = new InputChannelPacket(new ByteBuf(data[i]));
            try {
                ProcessInputChannelPacket(packet);
            } catch (Exception e) {
                Debug.LogError($"Error processing input channel packet:");
                Debug.LogException(e);
            }
            ret[i] = packet;
        }
        
        return ret;
    }

    private void ProcessInputChannelPacket(InputChannelPacket packet) {
        foreach (var subpacket in packet.subpackets) {
            if (subpacket is ChannelSubpacketCustom customSubPacket) {
                // check if we have already received this aux packets
                if (auxiliaryInputsReceived.Contains(customSubPacket.nonce)) continue;
                Debug.Log($"received aux input {customSubPacket}");
                callbackHandler.OnReceivedAuxiliaryData(customSubPacket);
                auxiliaryInputsReceived.Add(customSubPacket.nonce);
            }
        }
    }
    
    public SynchronizedInput GetRemoteInputSync(out GGPOStatusCode result) {
        var packets = ReadInputChannelSync(out result);
        if (packets == null || packets.Any(c => c == null)) return new();
        
        var localIndex = packets[0].playerId == room.localPlayer.playerId ? 0 : 1;
        var remoteIndex = localIndex == 0 ? 1 : 0;

        return new() {
            local = packets[localIndex].FindFirst<ChannelSubpacketInput>().inputChord,
            remote = packets[remoteIndex].FindFirst<ChannelSubpacketInput>().inputChord
        };
    }

    public GGPOStatusCode NotifyTickSync() {
        return (GGPOStatusCode)GGPO.Session.AdvanceFrame();
    }
    
    public GGPOStatusCode NotifyIdleSyncNonBlocking(int time) {
        return (GGPOStatusCode)GGPO.Session.Idle(time); 
    }
    
    // Called at the start of each frame
    public void Tick() {
        
    }
    
    private bool OnGameBegin(string text) {
        Debug.Log($"game begin, {text}");
        return true;
    }

    private bool OnAdvanceFrame(int flags) {
        var ticked = FightEngine.inst.TickGameStateGGPO();
        if (!ticked) GGPO.Session.AdvanceFrame();
        return true;
    }

    private bool OnLogGameState(string fileName, NativeArray<byte> data) {
        // Debug.Log($"log game state, size: {data.Length}");
        return true;
    }
    
    private bool OnSaveGameState(out NativeArray<byte> data, out int checksum, int frame) {
        ++gamestateHandleCounter;
        var saved = callbackHandler.OnSaveGameState(gamestateHandleCounter);
        // Debug.Log($"save game state, frame: {frame}, handle: {gamestateHandleCounter}");

        var buf = new ByteBuf(1 + 4 + 4);
        buf.SetByteAt(0, (byte)(!saved ? 0xff : 0x01));
        buf.SetDWordAt(1, (uint)gamestateHandleCounter);
        buf.SetDWordAt(5, (uint)frame);
        data = new NativeArray<byte>(buf.bytes, Allocator.Persistent);
        checksum = NetworkUtil.CalcFletcher32(data);
        return true;
    }
    
    private bool OnLoadGameState(NativeArray<byte> data) {
        if (data.Length == 0) {
            Debug.LogError($"Request game state load but data is empty????");
            return true;
        }

        var buf = new ByteBuf(data.ToArray());
        Debug.Log($"Load game state, {buf}, handle: {buf.GetDWordAt(1)}");
        if (buf.GetByteAt(0) == 0x01) {
            var handle = (int)buf.GetDWordAt(1);
            callbackHandler.OnLoadGameState(handle);
        }
        
        return true;
    }

    private void OnFreeBuffer(NativeArray<byte> data) {
        // Debug.Log($"free buffer");
        var buf = new ByteBuf(data.ToArray());
        var handle = (int)buf.GetDWordAt(1);
        
        callbackHandler.OnDeleteSavedGameState(handle);
    }
    
    private bool OnEventConnectedToPeer(int handle) {
        Debug.Log($"Connected to peer {handle}");
        if (status != GGPOConnectionStatus.CONNECTING) return false;
        status = GGPOConnectionStatus.CONNECTED_WAIT;
        return true;
    }
    
    private bool OnEventSynchronizingWithPeer(int handle, int count, int total) {
        Debug.Log($"Event synchronizing with peer {handle}: {count}/{total}");
        if (status != GGPOConnectionStatus.CONNECTED_WAIT) return false;
        status = GGPOConnectionStatus.SYNCHRONIZING;
        return true;
    }
    
    private bool OnEventSynchronizedWithPeer(int handle) {
        Debug.Log($"Synchronized with peer {handle}");
        if (status != GGPOConnectionStatus.SYNCHRONIZING) return false;
        status = GGPOConnectionStatus.SYNCHRONIZED;
        return true;
    }
    
    private bool OnEventRunning() {
        Debug.Log($"Good job! Game is running.");
        if (status != GGPOConnectionStatus.SYNCHRONIZED) return false;
        status = GGPOConnectionStatus.ESTABLISHED;
        return true;
    }
    
    private bool OnEventConnectionInterrupted(int handle, int disconnectTimeout) {
        Debug.LogWarning($"GGPO connection interrupted with peer {handle}");
        return true;
    }
    
    private bool OnEventConnectionResumed(int handle) {
        Debug.Log("Connection resumed");
        return true;
    }
    
    private bool OnEventDisconnectedFromPeer(int handle) {
        Debug.LogWarning($"GGPO disconnected from peer {handle}");
        onDisconnected.Invoke();
        status = GGPOConnectionStatus.DISCONNECTED;
        return true;
    }
    
    private bool OnEventEventcodeTimesync(int framesAhead) {
        Debug.Log($"GGPO timesync: {framesAhead} frames ahead");
        this.framesAhead = framesAhead;
        return true;
    }

    [AOT.MonoPInvokeCallback(typeof(GGPO.LogDelegate))]
    private static void OnLog(string msg) {
        Debug.Log($"[GGPO Connector] {msg}");
        
    }
    
    public void Dispose() {
        var res = (GGPOStatusCode)GGPO.Session.CloseSession();
        if (res != GGPOStatusCode.OK) {
            Debug.LogError($"Failed to close GGPO session: {res}, the underlying session object may have not been properly disposed. Client restart is recommended.");
            return;
        }
        
        Debug.Log($"GGPO session disposed");
        inst = null;
    }
}

public struct SynchronizedInput {
    public InputChord local;
    public InputChord remote;
}
}
