using System;
using SuperSmashRhodes.Network.Rollbit.P2P;

namespace SuperSmashRhodes.Network.Rollbit {
public enum PacketType : ushort {
    PLAY_IN_HANDSHAKE = 0x0001,
    PLAY_IN_HEARTBEAT = 0x0002,
    PLAY_IN_ACCEPT_MATCH = 0x0004,
    PLAY_IN_NEGOTIATE = 0x0005,
    PLAY_IN_CHARACTER_SELECT = 0x0007,
    PLAY_IN_CONFIRM_P2P = 0x0008,
    PLAY_IN_ROUND_STATUS = 0x0009,
    
    PLAY_IN_DISCONNECT = 0x00ff,
    
    PLAY_OUT_RESPONSE = 0xff00,
    PLAY_OUT_HANDSHAKE = 0xff01,
    PLAY_OUT_ROOM_STATUS = 0xff04,
    PLAY_OUT_PRE_RANDOM = 0xff05,
    PLAY_OUT_BEGIN_P2P = 0xff06,
    PLAY_OUT_CHARACTER_SELECT = 0xff07,
    
    PLAY_OUT_DISCONNECT = 0xffff,
    
    PLAY_P2P_HANDSHAKE = 0x7700,
    // PLAY_P2P_HEARTBEAT = 0x7701,
    PLAY_P2P_INPUT = 0x7702,
}

public static class PacketTypeExtensions {
    public static Type GetClass(this PacketType type) {
        return type switch {
            PacketType.PLAY_OUT_DISCONNECT => typeof(PacketPlayOutDisconnect),
            PacketType.PLAY_OUT_HANDSHAKE => typeof(PacketPlayOutHandshake),
            PacketType.PLAY_OUT_RESPONSE => typeof(PacketPlayOutGenericResponse),
            PacketType.PLAY_OUT_ROOM_STATUS => typeof(PacketPlayOutRoomStatus),
            PacketType.PLAY_OUT_PRE_RANDOM => typeof(PacketPlayOutPreRandom),
            PacketType.PLAY_OUT_BEGIN_P2P => typeof(PacketPlayOutBeginP2P),
            PacketType.PLAY_OUT_CHARACTER_SELECT => typeof(PacketPlayOutCharacterSelect),
            
            _ => null
        };
    }
}
}
