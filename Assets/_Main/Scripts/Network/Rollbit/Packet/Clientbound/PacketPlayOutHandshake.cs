using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayOutHandshake : ClientboundPacket{
    public uint heartbeatInterval { get; }
    
    public PacketPlayOutHandshake(NetworkSession session, PacketHeader header, ByteBuf body) : base(session, header, body) {
        heartbeatInterval = body.GetDWordAt(0);
    }
    public override PacketType type { get; } = PacketType.PLAY_OUT_HANDSHAKE;

    public override string ToString() {
        return $"PacketPlayOutHandshake(heartbeatInterval={heartbeatInterval})";
    }
}
}
