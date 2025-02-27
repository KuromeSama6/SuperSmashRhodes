using System;
using System.Collections;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Stage;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Room;
using SuperSmashRhodes.Scripts.Audio;
using SuperSmashRhodes.UI.LoadingScreen;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void BeginLocalMatch(RoomConfiguration config, StageData stageData, StageBGMData bgmData, PlayerMatchData p1, PlayerMatchData p2) {
        var room = new LocalRoom(config);
        room.players[0] = p1;
        room.players[1] = p2;
        room.stageData = stageData;
        room.bgmData = bgmData;
        CreateRoom(room);

        StartCoroutine(LoadMatchScene());
    }
    
    private void OnRoomDestroy() {
        current = null;
    }

    private IEnumerator LoadMatchScene() {
        var loadingScreen = LoadingScreen.inst;
        loadingScreen.visible = true;
        
        loadingScreen.SetLoadingChecklist(
            "玩家数据",
            "输入设备通讯",
            "对战场景",
            "场景初始化",
            "动态通用资源"
            );

        yield return new WaitForSeconds(1); // TODO remove placeholder loading
        loadingScreen.UpdateLoadingStatus(LoadingStatus.GOOD);
        yield return new WaitForSeconds(1); // TODO remove placeholder loading
        loadingScreen.UpdateLoadingStatus(LoadingStatus.GOOD);
        
        // load the scene
        var handle = SceneManager.LoadSceneAsync(current.stageData.sceneId);
        if (handle == null) {
            Debug.LogError("Failed to load scene");
            loadingScreen.UpdateLoadingStatus(LoadingStatus.BAD);
            yield break;
        }
        
        handle.allowSceneActivation = false;
        
        while (!handle.isDone) {
            if (handle.progress >= 0.9f) {
                loadingScreen.showCover = true;
                
                yield return new WaitForSeconds(1);
                handle.allowSceneActivation = true;
                loadingScreen.UpdateLoadingStatus(LoadingStatus.GOOD);
                break;
            }
            
            yield return null;
        }
        
        yield return new WaitForSeconds(1); // TODO remove placeholder loading
        loadingScreen.UpdateLoadingStatus(LoadingStatus.GOOD);

        GameManager.inst.PreloadResources();
        yield return new WaitForSeconds(4);
        AudioManager.inst.StopBGM(1f);
        loadingScreen.UpdateLoadingStatus(LoadingStatus.GOOD);
        
        // begin
        AudioManager.inst.PlayBGM(current.bgmData, gameObject, 1f, .2f);
        yield return new WaitForSeconds(3);
        loadingScreen.coverFadeSpeed = 5f;
        loadingScreen.visible = false;

        current.BeginMatch();
    }
}
}
