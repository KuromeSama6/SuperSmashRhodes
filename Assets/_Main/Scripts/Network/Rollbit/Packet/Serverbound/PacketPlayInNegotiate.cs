using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayInNegotiate : ServerboundPacket {
    private readonly int port;
    private readonly string localAddress;

    public PacketPlayInNegotiate(NetworkSession session, int port, string localAddress) : base(session) {
        this.port = port;
        this.localAddress = localAddress;
    }
    
    public override PacketType type => PacketType.PLAY_IN_NEGOTIATE;
    public override uint bodySize => 2 + 4;
    
    protected override void SerializeInternal(ByteBuf buf) {
        buf.SetWordAt(0, (ushort)port);
        // conver the local address to four bytes
        byte[] addressBytes = new byte[4];
        string[] addressParts = localAddress.Split('.');
        for (int i = 0; i < 4; i++) {
            addressBytes[i] = byte.Parse(addressParts[i]);
        }
        
        buf.SetBytes(2, addressBytes);
    }
}
}
