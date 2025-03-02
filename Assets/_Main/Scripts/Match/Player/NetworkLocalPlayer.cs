using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Config.Global;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Network.Rollbit;
using SuperSmashRhodes.Network.RoomManagement;
using UnityEngine;

namespace SuperSmashRhodes.Match.Player {
public class NetworkLocalPlayer : LocalPlayer { 
    public override LocalInputModule input => InputDevicePool.inst.defaultInput;
    private NetworkRoom networkRoom;

    public NetworkLocalPlayer(Room room, int playerId, string userId) : base(room, playerId, null) {
        this.userId = userId;
        networkRoom = (NetworkRoom)room;
    }

    public override void SetCharacter(CharacterDescriptor characterDescriptor) {
        base.SetCharacter(characterDescriptor);
        UpdateCharacterSelect();
    }

    public override void SetConfirmed(bool confirmed) {
        base.SetConfirmed(confirmed);
        UpdateCharacterSelect();
    }

    public void UpdateCharacterSelect() {
        if (!character) return;
        _ = networkRoom.session.SendPacket(new PacketPlayInCharacterSelect(networkRoom.session, this));
    }
    
}
}
