using SuperSmashRhodes.Network.RoomManagement;
using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayInRoundStatus : ServerboundPacket {
    private readonly NetworkRoom room;
    private readonly bool fighting;

    public PacketPlayInRoundStatus(NetworkSession session, bool fighting, NetworkRoom room) : base(session) {
        this.fighting = fighting;
        this.room = room;
    }
    public override PacketType type => PacketType.PLAY_IN_ROUND_STATUS;
    public override uint bodySize => 2 + 2 + 2;
    
    protected override void SerializeInternal(ByteBuf buf) {
        buf.SetByteAt(0, (byte)(fighting ? 1 : 0));
        buf.SetWordAt(2, (ushort)room.roundsWon);
        buf.SetWordAt(4, (ushort)room.roundsPlayed);
    }
}
}
