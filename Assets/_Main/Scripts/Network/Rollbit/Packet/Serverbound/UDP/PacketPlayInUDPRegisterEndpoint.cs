using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayInUDPRegisterEndpoint : ServerboundPacket {
    public PacketPlayInUDPRegisterEndpoint(NetworkSession session) : base(session) { }
    public override PacketType type => PacketType.PLAY_IN_UDP_REGISTER_ENDPOINT;
    public override uint bodySize => 0;
    protected override void SerializeInternal(ByteBuf buf) {
    }
}
}
