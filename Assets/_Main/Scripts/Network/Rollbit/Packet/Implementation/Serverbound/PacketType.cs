using System;

namespace SuperSmashRhodes.Network.Rollbit {
public enum PacketType : ushort {
    PLAY_IN_HANDSHAKE = 0x0001,
    PLAY_IN_HEARTBEAT = 0x0002,
    PLAY_IN_ACCEPT_MATCH = 0x0004,
    
    PLAY_IN_DISCONNECT = 0x00ff,
    
    PLAY_OUT_RESPONSE = 0xff00,
    PLAY_OUT_HANDSHAKE = 0xff01,
    PLAY_OUT_ROOM_STATUS = 0xff04,
    
    PLAY_OUT_DISCONNECT = 0xffff
}

public static class PacketTypeExtensions {
    public static Type GetClass(this PacketType type) {
        return type switch {
            PacketType.PLAY_OUT_DISCONNECT => typeof(PacketPlayOutDisconnect),
            PacketType.PLAY_OUT_HANDSHAKE => typeof(PacketPlayOutHandshake),
            PacketType.PLAY_OUT_RESPONSE => typeof(PacketPlayOutGenericResponse),
            PacketType.PLAY_OUT_ROOM_STATUS => typeof(PacketPlayOutRoomStatus),
            _ => null
        };
    }
}
}
