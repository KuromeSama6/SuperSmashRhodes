using System.Collections;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Stage;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Match.Player;
using SuperSmashRhodes.Scripts.Audio;
using SuperSmashRhodes.UI.Global.LoadingScreen;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SuperSmashRhodes.Network.RoomManagement {
public class LocalRoom : Room {
    public LocalRoom(RoomConfiguration config) : base(config) {
        status = RoomStatus.CHARACTER_SELECT;
    }

    public LocalPlayer AddLocalPlayer(string inputDeviceName) {
        var player = new LocalPlayer(this, freePlayerId, inputDeviceName);
        AddPlayer(player);
        return player;
    }
    
    public void BeginMatch(StageData stageData, StageBGMData bgmData, bool skipLoading = false) {
        this.stageData = stageData;
        this.bgmData = bgmData;

        status = RoomStatus.CLIENT_LOADING;
        
        if (skipLoading) {
            StartMatch();
        } else {
            StartCoroutine(LoadLocalMatchRoutine());
        }
    }

    private IEnumerator LoadLocalMatchRoutine() {
        yield return LoadMatchSceneRoutine();
        StartMatch();
    }
}
}
