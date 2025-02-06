using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayOutRoomStatus : ClientboundPacket{
    public RoomStatus status {get; private set;}
    public int playerCount {get; private set;}
    public int spectatorCount {get; private set;}
    
    public int totalPlayers => playerCount + spectatorCount;
    
    public PacketPlayOutRoomStatus(NetworkSession session, PacketHeader header, ByteBuf body) : base(session, header, body) {
        status = (RoomStatus)body.GetWordAt(0);
        playerCount = body.GetWordAt(2);
        spectatorCount = body.GetWordAt(4);
    }
    
    public override PacketType type { get; } = PacketType.PLAY_OUT_ROOM_STATUS;
}

public enum RoomStatus : ushort {
    STANDBY = 0,
    WAIT_ACCEPT = 1,
    CHARACTER_SELECT = 2,
    NEGOTIATING = 3,
    CLIENT_LOADING = 4,
    PLAYING = 5,
    FINISHED = 6
}
}
