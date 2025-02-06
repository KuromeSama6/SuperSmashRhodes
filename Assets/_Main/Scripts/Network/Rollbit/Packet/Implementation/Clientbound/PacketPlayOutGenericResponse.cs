using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayOutGenericResponse : ClientboundPacket{
    public PacketPlayOutGenericResponse(NetworkSession session, PacketHeader header, ByteBuf body) : base(session, header, body) {
        
    }
    
    public override PacketType type { get; } = PacketType.PLAY_OUT_RESPONSE;
}
}
