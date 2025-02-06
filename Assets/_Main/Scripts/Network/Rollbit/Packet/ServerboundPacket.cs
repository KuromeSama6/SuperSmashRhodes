using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public abstract class ServerboundPacket : Packet {
    public abstract uint bodySize { get; }

    public PacketHeader header {
        get {
            return new PacketHeader(type, session.config.version, bodySize + 32, bodySize, 0, 0, session.config.userId);
        }
    }
    
    public ServerboundPacket(NetworkSession session) : base(session) {
        
    }

    public ByteBuf Serialize() {
        var buf = new ByteBuf(bodySize);
        SerializeInternal(buf);
        return buf.Copy();
    }

    protected abstract void SerializeInternal(ByteBuf buf);

}
}
