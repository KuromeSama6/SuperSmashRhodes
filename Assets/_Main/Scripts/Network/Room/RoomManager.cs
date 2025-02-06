using System;
using SuperSmashRhodes.Network.Rollbit;
using UnityEngine;

namespace SuperSmashRhodes.Network.Room {
public class RoomManager : MonoBehaviour, IPacketHandler {
    public RoomStatus status { get; private set; } = RoomStatus.STANDBY;
    public int playerCount { get; private set; } = 0;
    public int spectatorCount { get; private set; } = 0;
    public bool matchAccepted { get; private set; }
    
    private NetworkSession session;

    private void Start() {
        
    }

    public void Init(NetworkSession session) {
        this.session = session;
        session.AddPacketHandler(this);
        this.session.onDisconnected.AddListener(OnDisconnected);
    }
    
    public void NotifyMatchAccept(bool accepted) {
        if (status != RoomStatus.WAIT_ACCEPT || matchAccepted) return;

        session.SendPacket<PacketPlayOutGenericResponse>(new PacketPlayInAcceptMatch(session, accepted))
            .ContinueWith(p => {
                matchAccepted = p.Result.header.statusCode == 0;
            });
    }
    
    private void OnDisconnected() {
        
    }
    
    // Packet handlers
    [PacketHandler]
    public void OnRoomStatusUpdate(PacketPlayOutRoomStatus packet) {
        print($"Room status update: {packet.status}");
        status = packet.status;
        playerCount = packet.playerCount;
        spectatorCount = packet.spectatorCount;
    }
}
}
