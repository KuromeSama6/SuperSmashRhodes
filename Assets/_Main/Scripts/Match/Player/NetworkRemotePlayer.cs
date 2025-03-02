using SuperSmashRhodes.Config.Global;
using SuperSmashRhodes.Network.RoomManagement;

namespace SuperSmashRhodes.Match.Player {
public class NetworkRemotePlayer : Player {
    public NetworkRemotePlayer(Room room, int playerId, string userId) : base(room, playerId, userId) {
    }
}
}
