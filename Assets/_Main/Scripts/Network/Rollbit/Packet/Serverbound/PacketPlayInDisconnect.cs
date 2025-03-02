using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayInDisconnect : ServerboundPacket{
    public ClientDisconnectionReason reason { get; private set; }
    public string remarks { get; private set; }
    
    public PacketPlayInDisconnect(NetworkSession session, ClientDisconnectionReason reason, string remarks) : base(session) {
        this.reason = reason;
        this.remarks = remarks;
    }
    
    public override PacketType type { get; } = PacketType.PLAY_IN_DISCONNECT;
    public override uint bodySize => 2 + (uint)remarks.Length + 1;
    protected override void SerializeInternal(ByteBuf buf) {
        buf.SetWordAt(0, (ushort)reason);
        buf.SetStringNT(2, remarks);
    }
}

public enum ClientDisconnectionReason : ushort {
    UNKNOWN = 0,
    BACK_TO_LOBBY = 1,
    CONNECTION_RESET = 2,
    PROTOCOL_ERROR = 3,
    CLIENT_ERROR = 4 
}
}
