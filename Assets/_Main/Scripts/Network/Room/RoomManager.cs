using System;
using SuperSmashRhodes.Framework;

namespace SuperSmashRhodes.Network.Room {
public class RoomManager : AutoInitSingletonBehaviour<RoomManager> {
    public Room current { get; private set; }

    private void Start() {
        
    }

    public void CreateRoom(Room room) {
        if (current != null) return;
        current = room;
        
        room.onDestroy.AddListener(OnRoomDestroy);
    }

    public void CloseRoom() {
        if (current == null) return;
        current.Destroy();
    }

    private void OnRoomDestroy() {
        current = null;
    }
}
}
