using SuperSmashRhodes.Config.Global;
using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit.P2P {
public class PacketPlayP2PHandhsake : P2PPacket {
    public PacketPlayP2PHandhsake(P2PConnector connector, PacketHeader header, ByteBuf body) : base(connector, header, body) {
        
    }

    public PacketPlayP2PHandhsake(P2PConnector connector) : base(connector) {
        
    }

    public override PacketType type => PacketType.PLAY_P2P_HANDSHAKE;
    
    public override uint bodySize { get; }
    protected override void SerializeInternal(ByteBuf buf) {
        throw new System.NotImplementedException();
    }
}
}
