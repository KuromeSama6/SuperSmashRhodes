using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayOutDisconnect : ClientboundPacket {
    public ServerDisconnectionReason code { get; private set; }
    public string remarks { get; private set; }
    
    public override PacketType type { get; } = PacketType.PLAY_OUT_DISCONNECT;
    public PacketPlayOutDisconnect(NetworkSession session, PacketHeader header, ByteBuf body) : base(session, header, body) {
        code = (ServerDisconnectionReason)body.GetWordAt(0);
        remarks = body.GetStringNT(2, (int)header.bodyLength);
    }
}

public enum ServerDisconnectionReason : ushort {
    CLIENT_DISCONNECT = 0,
    UNKNOWN_ERROR = 1,
    MALFORMED_PACKET = 2,
    NOT_FOUND = 3,
    VERSION_MISMATCH = 4,
    INVALID_HEADERS = 5,
    LENGTH_MISMATCH = 6,
    HANDSHAKE_TIMEOUT = 7,
    TIMEOUT = 8,
    SEE_REMARKS = 15,
    PERMISSION_DENIED = 16,
    
    ROOM_FULL = 20,
    MATCH_DECLINED = 21,
    
    NEGOTIATION_FALIURE = 66
}
}