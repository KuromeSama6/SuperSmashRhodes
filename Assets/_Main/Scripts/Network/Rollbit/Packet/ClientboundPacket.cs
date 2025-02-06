using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public abstract class ClientboundPacket : Packet {
    public PacketHeader header { get; private set; }
    public ByteBuf body { get; private set; }
    
    public ClientboundPacket(NetworkSession session, PacketHeader header, ByteBuf body) : base(session) {
        this.header = header;
        this.body = body;
    }
}
}
