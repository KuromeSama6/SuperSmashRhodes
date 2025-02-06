using System;
using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayInHandshake : ServerboundPacket {
    public PlayerRole role { get; private set; }
    public PacketPlayInHandshake(NetworkSession session, PlayerRole role) : base(session) {
        this.role = role;
    }
    
    public override PacketType type { get; } = PacketType.PLAY_IN_HANDSHAKE;
    public override uint bodySize { get; } = 18;
    protected override void SerializeInternal(ByteBuf buf) {
        // ut timestamp ms
        buf.SetByteAt(0, (byte)role);
        buf.SetQWordAt(2, (ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds());
    }
}
}
