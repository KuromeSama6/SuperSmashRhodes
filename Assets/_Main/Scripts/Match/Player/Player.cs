using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Network.RoomManagement;

namespace SuperSmashRhodes.Match.Player {
/// <summary>
/// Represents a player in a room. A player object is created for each player within a room,
/// and is destroyed when the player leaves the room or the room is destroyed.
/// </summary>
public abstract class Player {
    public int playerId { get; private set; }
    public string userId { get; protected set; }
    public CarriedRoundData carriedData { get; set; }
    public CharacterDescriptor character { get; set; }
    public bool characterConfirmed { get; set; }
    public Room room { get; private set; }

    public Player(Room room, int playerId, string userId) {
        this.room = room;
        this.playerId = playerId;
        this.userId = userId;
    }
    
    public void SetCarriedData(CarriedRoundData data) {
        carriedData = data;
    }
    
}

public struct CarriedRoundData {
    public float carriedBurst;
}
}
