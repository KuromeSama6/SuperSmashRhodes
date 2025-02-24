using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Input;

namespace SuperSmashRhodes.Room {
public class CharacterSelectData {
    public string inputDeviceName { get; private set; }
    public CharacterDescriptor selectedCharacter { get; set; }

    public LocalInputModule input => InputDevicePool.inst.inputs[inputDeviceName];
    
    public CharacterSelectData(LocalInputModule input) {
        inputDeviceName = input.input.currentControlScheme;
        
    }
}
}
