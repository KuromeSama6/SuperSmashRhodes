using System;
using System.Collections.Generic;
using SuperSmashRhodes.Match.Player;
using SuperSmashRhodes.Network;
using SuperSmashRhodes.Network.Rollbit;
using SuperSmashRhodes.Network.Rollbit.P2P;
using SuperSmashRhodes.Network.RoomManagement;
using SuperSmashRhodes.Util;
using Unity.Collections;
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

    public bool connected => status == GGPOConnectionStatus.ESTABLISHED || status == GGPOConnectionStatus.SYNCHRONIZING;
    
    public GGPOConnector(NetworkRoom room) {
        if (inst != null) {
            throw new InvalidOperationException("The last GGPOConnector instance has not been disposed yet. It must be disposed before creating a new one.");
        }

        this.room = room;
        inst = this;
    }

    public bool Bind() {
        if (status != GGPOConnectionStatus.STANDBY) {
            throw new InvalidOperationException("The GGPOConnector is already bound.");
        }

        port = room.localPlayer.playerId == 0 ? room.session.config.ggpoPortP1 : room.session.config.ggpoPortP2;

        if (GGPO.Session.IsStarted()) {
            GGPO.Session.CloseSession();   
        }
        
        GGPOStatusCode res = (GGPOStatusCode)GGPO.Session.StartSession(OnGameBegin, OnAdvanceFrame, OnLoadGameState, OnLogGameState, OnSaveGameState, OnFreeBuffer, OnEventConnectedToPeer, OnEventSynchronizingWithPeer, OnEventSynchronizedWithPeer, OnEventRunning, OnEventConnectionInterrupted, OnEventConnectionResumed, OnEventDisconnectedFromPeer, OnEventEventcodeTimesync, "ssr_networkgame", 2, port);

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
        return true;
    }

    public GGPOStatusCode AddLocalInputSync(int playerId, byte[] data) {
        // pad to max size
        if (!playerIdToHandleMap.ContainsKey(playerId)) return GGPOStatusCode.OK;
        var buffer = new byte[INPUT_BUFFER_SIZE];
        Array.Copy(data, 0, buffer, 0, data.Length);
        return (GGPOStatusCode)GGPO.Session.AddLocalInput(playerIdToHandleMap[playerId], INPUT_BUFFER_SIZE, buffer);
    }

    public byte[] GetRemoteInputSync(out GGPOStatusCode result) {
        var data = GGPO.Session.SynchronizeInput(2, INPUT_BUFFER_SIZE, out int res, out int disconnectFlags);
        result = (GGPOStatusCode)res;
        return data[0];
    }

    public GGPOStatusCode NotifyTickSync() {
        return (GGPOStatusCode)GGPO.Session.AdvanceFrame();
    }
    
    public GGPOStatusCode NotifyIdleSyncNonBlocking(float deltaTime) {
        return (GGPOStatusCode)GGPO.Session.Idle(Mathf.RoundToInt(deltaTime * 1000f));
    }
    
    // Called at the start of each frame
    public void Tick() {
        
    }
    
    private bool OnGameBegin(string text) {
        Debug.Log($"game begin, {text}");
        return true;
    }

    private bool OnAdvanceFrame(int flags) {
        return true;
    }

    private bool OnLoadGameState(NativeArray<byte> data) {
        return true;
    }

    private bool OnLogGameState(string fileName, NativeArray<byte> data) {
        return true;
    }
    
    private bool OnSaveGameState(out NativeArray<byte> data, out int checksum, int frame) {
        checksum = 0;
        data = new NativeArray<byte>(0, Allocator.Temp);
        return true;
    }

    private void OnFreeBuffer(NativeArray<byte> data) {
        
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
}
