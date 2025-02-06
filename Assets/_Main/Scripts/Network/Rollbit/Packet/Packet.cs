namespace SuperSmashRhodes.Network.Rollbit {
public abstract class Packet {
    public NetworkSession session { get; private set; }
    public abstract PacketType type { get; }
    
    public Packet(NetworkSession session) {
        this.session = session;
    }
}
}
