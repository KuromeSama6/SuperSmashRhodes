using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayInConfirmP2P : ServerboundPacket {
    private readonly int status;
    private readonly ulong nonce;
    
    public PacketPlayInConfirmP2P(NetworkSession session, int status, ulong nonce) : base(session) {
        this.status = status;
        this.nonce = nonce;
    }
    
    public override PacketType type => PacketType.PLAY_IN_CONFIRM_P2P;
    public override uint bodySize => 2 + 8;
    
    protected override void SerializeInternal(ByteBuf buf) {
        buf.SetByteAt(0, (byte)status);
        buf.SetQWordAt(2, nonce);
    }
}
}
