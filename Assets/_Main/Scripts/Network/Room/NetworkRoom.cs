using System;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Network.Rollbit;
using UnityEngine;

namespace SuperSmashRhodes.Network.Room {
public class NetworkRoom : Room, IPacketHandler {
    public bool matchAccepted { get; private set; }
    
    private NetworkSession session;

    public NetworkRoom(RoomConfiguration configuration, NetworkSession session) : base(configuration) {
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
        Debug.LogWarning("Disconnected from server, destroying room");
        Destroy();
    }
    
    // Packet handlers
    [PacketHandler]
    public void OnRoomStatusUpdate(PacketPlayOutRoomStatus packet) {
        Debug.Log($"Room status update: {packet.status}");
        status = packet.status;
        playerCount = packet.playerCount;
        spectatorCount = packet.spectatorCount;
    }
}
}
