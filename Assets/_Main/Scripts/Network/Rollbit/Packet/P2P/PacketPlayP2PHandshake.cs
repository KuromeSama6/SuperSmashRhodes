using SuperSmashRhodes.Config.Global;
using SuperSmashRhodes.Network.Rollbit.P2P;
using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayP2PHandshake : P2PPacket {
    public readonly ulong nonce;
    public readonly ulong verifier;
    public readonly int heartbeatInterval;
    
    public PacketPlayP2PHandshake(P2PConnector connector, PacketHeader header, ByteBuf body) : base(connector, header, body) {
        nonce = body.GetQWordAt(0);
        verifier = body.GetQWordAt(8);
        heartbeatInterval = body.GetWordAt(16);
    }

    public PacketPlayP2PHandshake(P2PConnector connector, PacketPlayOutBeginP2P negotiationPacket) : base(connector) {
        nonce = negotiationPacket.nonce;
        verifier = negotiationPacket.verifier;
        heartbeatInterval = 5000;
    }

    public override PacketType type => PacketType.PLAY_P2P_HANDSHAKE;

    public override uint bodySize => 8 + 8 + 8;
    protected override void SerializeInternal(ByteBuf buf) {
        buf.SetQWordAt(0, nonce);
        buf.SetQWordAt(8, verifier);
        buf.SetWordAt(16, (ushort)heartbeatInterval);
    }

    public override string ToString() {
        return $"PacketPlayP2PHandshake(nonce={nonce}, verifier={verifier})";
    }
}
}
