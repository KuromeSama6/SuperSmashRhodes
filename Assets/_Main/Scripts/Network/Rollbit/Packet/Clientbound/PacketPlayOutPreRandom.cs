using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayOutPreRandom : ClientboundPacket {
    public readonly double[] randoms;

    public PacketPlayOutPreRandom(NetworkSession session, PacketHeader header, ByteBuf body) : base(session, header, body) {
        var size = (int)body.GetDWordAt(0);
        randoms = new double[size];
        
        for (var i = 0; i < size; i++) {
            randoms[i] = body.GetDoubleAt(4 + i * 8);
        }
    }
    
    public override PacketType type => PacketType.PLAY_OUT_PRE_RANDOM;

    public override string ToString() {
        return $"PacketPlayOutPreRandom(length={randoms.Length})";
    }
}
}
