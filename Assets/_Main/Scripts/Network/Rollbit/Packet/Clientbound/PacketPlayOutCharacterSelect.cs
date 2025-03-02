using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayOutCharacterSelect : ClientboundPacket {
    public readonly string userId;
    public readonly int index;
    public readonly bool confirmed;
    
    public PacketPlayOutCharacterSelect(NetworkSession session, PacketHeader header, ByteBuf body) : base(session, header, body) {
        userId = body.GetStringNT(0, 16);
        index = body.GetWordAt(16);
        confirmed = body.GetByteAt(18) == 1;
    }
    
    public override PacketType type => PacketType.PLAY_OUT_CHARACTER_SELECT;
}
}
