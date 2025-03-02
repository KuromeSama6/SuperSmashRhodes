using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Input;

namespace SuperSmashRhodes.Match {
internal class PlayerMatchData {
    public string inputDeviceName { get; private set; }
    public CharacterDescriptor character { get; set; }
    public bool confirmed { get; set; }
    public bool isCpu { get; set; }
    public int playerId { get; set; }
    
    
    private PlayerMatchData() {}
    
    public PlayerMatchData(int playerId, LocalInputModule input) {
        inputDeviceName = input.input.currentControlScheme;
        this.playerId = playerId;
    }

    public static PlayerMatchData CreateDebugPlayerData(int playerId, string deviceName, CharacterDescriptor character) {
        var data = new PlayerMatchData();
        data.playerId = playerId;
        data.inputDeviceName = deviceName;
        data.character = character;
        data.confirmed = true;
        return data;
    }
}
}
