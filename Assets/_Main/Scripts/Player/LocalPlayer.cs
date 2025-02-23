namespace SuperSmashRhodes.Player {
public class LocalPlayer {
    public string inputDevice { get; private set; }
    public int playerIndex { get; private set; }
    
    public LocalPlayer(string inputDevice, int playerIndex) {
        this.inputDevice = inputDevice;
        this.playerIndex = playerIndex;
    }
}
}
