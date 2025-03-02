using System.Collections.Generic;
using SuperSmashRhodes.Network.RoomManagement;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayOutRoomStatus : ClientboundPacket{
    public RoomStatus status { get; private set; }
    public string[] userIds { get; private set; }

    public PacketPlayOutRoomStatus(NetworkSession session, PacketHeader header, ByteBuf body) : base(session, header, body) {
        status = (RoomStatus)body.GetWordAt(0);
        userIds = new string[body.GetWordAt(2)];
        
        // Debug.Log(body.ToString());
        // Debug.Log(status);
        // Debug.Log(playerIds.Length);
        
        for (int i = 0; i < userIds.Length; i++) {
            // Debug.Log($"start: {4 + i * 16}, end: {4 + i * 16 + 16}");
            userIds[i] = body.GetStringNT(4 + i * 16, 16);
        }
    }
    
    public override PacketType type { get; } = PacketType.PLAY_OUT_ROOM_STATUS;
}


}
