using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Network.RoomManagement;

namespace SuperSmashRhodes.Match.Player {
/// <summary>
/// Represents a player that is controller by a local input device.
/// </summary>
public class LocalPlayer : Player {
    public string inputDeviceName { get; private set; }
    
    public virtual LocalInputModule input => InputDevicePool.inst.inputs.GetValueOrDefault(inputDeviceName);
    
    public LocalPlayer(Room room, int playerId, string inputDeviceName) : base(room, playerId, $"local${playerId}") {
        this.inputDeviceName = inputDeviceName;
    }

    public virtual void SetCharacter(CharacterDescriptor characterDescriptor) {
        character = characterDescriptor;
    }
    
    public virtual void SetConfirmed(bool confirmed) {
        characterConfirmed = confirmed;
    }
}
}
