using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Input;

namespace SuperSmashRhodes.Room {
public class PlayerMatchData {
    public string inputDeviceName { get; private set; }
    public CharacterDescriptor selectedCharacter { get; set; }
    public bool confirmed { get; set; }
    public bool isCpu { get; set; }
    public int playerId { get; set; }
    
    public LocalInputModule input => InputDevicePool.inst.inputs.GetValueOrDefault(inputDeviceName);
    
    private PlayerMatchData() {}
    
    public PlayerMatchData(int playerId, LocalInputModule input) {
        inputDeviceName = input.input.currentControlScheme;
        this.playerId = playerId;
    }

    public static PlayerMatchData CreateDebugPlayerData(int playerId, string deviceName, CharacterDescriptor character) {
        var data = new PlayerMatchData();
        data.playerId = playerId;
        data.inputDeviceName = deviceName;
        data.selectedCharacter = character;
        data.confirmed = true;
        return data;
    }
}
}
