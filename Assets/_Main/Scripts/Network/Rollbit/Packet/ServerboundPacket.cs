using SuperSmashRhodes.Config.Global;
using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public abstract class ServerboundPacket : Packet {
    public abstract uint bodySize { get; }

    public PacketHeader header => CreateHeader(session.config.userId);
    
    public ServerboundPacket(NetworkSession session) : base(session) {
        
    }

    public ByteBuf Serialize() {
        var buf = new ByteBuf(bodySize);
        SerializeInternal(buf);
        return buf.Copy();
    }
    
    public PacketHeader CreateHeader(string userId) {
        return new PacketHeader(type, PacketHeader.ROLLBIT_MAGIC, ApplicationGlobalSettings.inst.rollbitVersion, bodySize + 32, bodySize, 0, 0, userId);
    }

    protected abstract void SerializeInternal(ByteBuf buf);

}
}
