using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayInAcceptMatch : ServerboundPacket {
    public bool accepted { get; private set; }
    
    public PacketPlayInAcceptMatch(NetworkSession session, bool accepted) : base(session) {
        this.accepted = accepted;
    }
    public override PacketType type { get; } = PacketType.PLAY_IN_ACCEPT_MATCH;
    public override uint bodySize => 1;
    protected override void SerializeInternal(ByteBuf buf) {
        buf.SetByteAt(0, accepted ? (byte)0 : (byte)1);
    }
}
}
