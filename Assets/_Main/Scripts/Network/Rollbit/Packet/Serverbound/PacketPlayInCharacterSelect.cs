using SuperSmashRhodes.Config.Global;
using SuperSmashRhodes.Match.Player;
using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayInCharacterSelect : ServerboundPacket {
    private readonly Player player;
    
    public PacketPlayInCharacterSelect(NetworkSession session, Player player) : base(session) {
        this.player = player;
    }
    
    public override PacketType type => PacketType.PLAY_IN_CHARACTER_SELECT;
    public override uint bodySize => 2 + 1;
    
    protected override void SerializeInternal(ByteBuf buf) {
        buf.SetWordAt(0, (ushort)player.character.characterIndex);
        buf.SetByteAt(2, player.characterConfirmed ? (byte)1 : (byte)0);
    }
}
}
