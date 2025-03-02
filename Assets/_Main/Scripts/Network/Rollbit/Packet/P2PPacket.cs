using SuperSmashRhodes.Config.Global;
using SuperSmashRhodes.Network.Rollbit.P2P;
using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public abstract class P2PPacket : Packet {
    public P2PConnector connector { get; private set; }
    public P2PPacketDirection direction { get; private set; }
    
    public abstract uint bodySize { get; }

    public PacketHeader header {
        get {
            return new PacketHeader(type, PacketHeader.P2P_MAGIC, ApplicationGlobalSettings.inst.rollbitVersion, bodySize + 32, bodySize, 0, 0, session.config.userId);
        }
    }

    public P2PPacket(P2PConnector connector, PacketHeader header, ByteBuf body) : base(connector.networkSession) {
        this.connector = connector;
        direction = P2PPacketDirection.INBOUND;
    }

    public P2PPacket(P2PConnector connector) : base(connector.networkSession) {
        this.connector = connector;
        direction = P2PPacketDirection.OUTBOUND;
    }

    public ByteBuf Serialize() {
        var buf = new ByteBuf(bodySize);
        SerializeInternal(buf);
        return buf.Copy();
    }

    protected abstract void SerializeInternal(ByteBuf buf);
    
}

public enum P2PPacketDirection {
    INBOUND,
    OUTBOUND
}
}
