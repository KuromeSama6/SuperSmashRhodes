using System;
using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayInHeartbeat : ServerboundPacket {
    public PacketPlayInHeartbeat(NetworkSession session) : base(session) {
        
    }
    public override PacketType type { get; } = PacketType.PLAY_IN_HEARTBEAT;
    public override uint bodySize { get; } = 8;
    protected override void SerializeInternal(ByteBuf buf) {
        buf.SetQWordAt(0, (ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds());
    }
}
}
