using System;
using System.Collections;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Stage;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Scripts.Audio;
using SuperSmashRhodes.UI.Global.LoadingScreen;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SuperSmashRhodes.Network.RoomManagement {
public class RoomManager : AutoInitSingletonBehaviour<RoomManager> {
    public static Room current { get; private set; }

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
