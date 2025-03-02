using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayInNegotiate : ServerboundPacket {
    private readonly int port;

    public PacketPlayInNegotiate(NetworkSession session, int port) : base(session) {
        this.port = port;
    }
    
    public override PacketType type => PacketType.PLAY_IN_NEGOTIATE;
    public override uint bodySize => 2;
    
    protected override void SerializeInternal(ByteBuf buf) {
        buf.SetWordAt(0, (ushort)port);
    }
}
}
