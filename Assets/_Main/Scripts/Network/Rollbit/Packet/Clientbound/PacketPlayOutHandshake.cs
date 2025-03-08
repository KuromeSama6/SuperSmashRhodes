using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayOutHandshake : ClientboundPacket{
    public uint heartbeatInterval { get; }
    public byte[] udpAddress { get; }
    public int udpPort { get; }
    public string udpToken { get; }
    
    public string udpAddressString => $"{udpAddress[0]}.{udpAddress[1]}.{udpAddress[2]}.{udpAddress[3]}";
    
    public PacketPlayOutHandshake(NetworkSession session, PacketHeader header, ByteBuf body) : base(session, header, body) {
        heartbeatInterval = body.GetDWordAt(0);
        udpAddress = body.GetBytes(4, 4);
        udpPort = body.GetWordAt(8);
        udpToken = body.GetStringNT(10, 16);
    }
    public override PacketType type { get; } = PacketType.PLAY_OUT_HANDSHAKE;

    public override string ToString() {
        return $"PacketPlayOutHandshake(heartbeatInterval={heartbeatInterval}, udpAddress={udpAddressString}, udpPort={udpPort}, udpToken={udpToken})";
    }
}
}
