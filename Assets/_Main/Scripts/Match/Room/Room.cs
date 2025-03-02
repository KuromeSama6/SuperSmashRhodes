using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Postprocess;
using SuperSmashRhodes.Battle.Stage;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Network.Rollbit;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Match.Player;
using SuperSmashRhodes.Scripts.Audio;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using SuperSmashRhodes.UI.Battle.Results;
using SuperSmashRhodes.UI.Generic;
using SuperSmashRhodes.UI.Global.LoadingScreen;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

namespace SuperSmashRhodes.Network.RoomManagement {
public abstract class Room {
    public RoomConfiguration config { get; private set; }
    public int playerCount { get; protected set; } = 0;
    public int spectatorCount { get; protected set; } = 0;
    public StageData stageData { get; protected set; }
    public StageBGMData bgmData { get; protected set; }
    
    public UnityEvent onDestroy { get; } = new();
    public UnityEvent<RoomStatus> onStatusChange { get; } = new();
    public Dictionary<int, Player> players { get; } = new();
    public float time { get; protected set; } = 999;
    public int round { get; protected set; }
    public List<RoundResult> results { get; } = new();
    
    private RoomStatus _status = RoomStatus.STANDBY;
    private bool unmanaged = false;
    
    public RoomStatus status {
        get => _status;
        protected set {
            if (value != status) {
                onStatusChange.Invoke(value);
            }
            _status = value;
        }
    }
    private GameManager gm => GameManager.inst;
    public int freePlayerId {
        get {
            if (!players.ContainsKey(0)) return 0;
            if (!players.ContainsKey(1)) return 1;
            return -1;
        }
    }
    public virtual bool allConfirmed => players.Count == playerCount && players.Values.All(p => p.characterConfirmed);
    
    public Room(RoomConfiguration config) {
        this.config = config;
        playerCount = 2;
    }

    public void Destroy() {
        onDestroy.Invoke();
        onDestroy.RemoveAllListeners();
    }

    protected void StartMatch() {
        status = RoomStatus.PLAYING;
        StartCoroutine(MatchRoutine());
        round = 0;
        time = config.roundTime;
        results.Clear();
        BattleTimer.inst.counter.ApplyImmediately();
    }

    public void EndRound() {
        
    }
    
    public void Tick() {
        if (unmanaged && time >= 0 && !config.infiniteTime && !gm.globalStateFlags.HasFlag(CharacterStateFlag.GLOBAL_PAUSE_TIMER)) {
            time -= Time.fixedDeltaTime;
            if (time <= 0) {
                EndRound();
                time = 0;
            }
        }
    }

    public void UpdateBadges() {
        foreach (var player in players.Values) {
            var badges = new List<RoundCompletionStatus>();
            var results = this.results.FindAll(c => c.winner.playerId == player.playerId);
            for (int i = 0; i < config.winRounds; i++) {
                if (i < results.Count) {
                    var result = results[i];
                    badges.Add(result.completionStatus);
                    
                } else {
                    badges.Add(RoundCompletionStatus.UNKNOWN);
                }
            }
            
            TopCharacterInfo.Get(player.playerId).SetBadges(badges);
        }
    }

    public void EndRound(PlayerCharacter winner) {
        var status = winner.healthPercent >= 1f ? RoundCompletionStatus.PERFECT : RoundCompletionStatus.COMPLETE;
        var winnerPlayer = players[winner.playerIndex];
        results.Add(new RoundResult(winnerPlayer, status));
        
        var isGameEnd = results.Count(c => c.winner.playerId == winnerPlayer.playerId) >= config.winRounds;
        // Debug.Log(results.Count(c => c.winner.playerId == winnerPlayer.playerId));
        unmanaged = false;
        
        StartCoroutine(RoundEndRoutine(winner, status, isGameEnd));
        BattleUIManager.inst.animator.SetBool("Preround", true);
        
        foreach (var player in gm.players.Values) {
            SaveCarriedData(player);
        }
    }

    private void LoadAndBeginNewRound() {
        GameManager.inst.ResetRound();
        
        StartCoroutine(BeginNewRoundRoutine());
    }
    

    private void RoundInit() {
        ++round;
        gm.inGame = false;
        
        BattleUIManager.inst.animator.SetTrigger("Handover");
        BattleUIManager.inst.animator.SetBool("Preround", false);
        gm.extraGlobalStateFlags = CharacterStateFlag.MANAGED;
        time = config.infiniteTime ? 999 : config.roundTime;
        
        BattleTimer.inst.counter.ApplyImmediately();
        
        foreach (var player in gm.players.Values) {
            player.BeginLogic();
        }
        
        UpdateBadges();
    }

    public int GetWinCount(int playerId) {
        return results.Count(c => c.winner.playerId == playerId);
    }
    
    public Player GetPlayer(int playerId) {
        return players.GetValueOrDefault(playerId);
    }
    
    protected void AddPlayer(Player player) {
        players[player.playerId] = player;
        playerCount = players.Count;
    }
    
    public void RemovePlayer(int playerId) {
        players.Remove(playerId);
    }
    
    private void SaveCarriedData(PlayerCharacter player) {
        var data = players[player.playerIndex];
        
        var carriedData = new CarriedRoundData {
            carriedBurst = player.burst.gauge.value
        };
        data.carriedData = carriedData;
    }
    
    protected void StartCoroutine(IEnumerator routine) {
        MainThreadDispatcher.RunOnMain(() => {
            RoomManager.inst.StartCoroutine(routine);
        });
    }
    
    //region Routines
    private IEnumerator MatchRoutine() {
        yield return GameEntryCinematicRoutine();
        
        // start logic
        RoundInit();
        yield return RoundStartRoutine();
        unmanaged = true;
        gm.inGame = true;
        
        GameStateManager.inst.RefreshComponentReferences();
    }
    
    private IEnumerator GameEntryCinematicRoutine() {
        yield return BeginMatchCoroutine();
    }
    
    private IEnumerator BeginMatchCoroutine() {
        gm.extraGlobalStateFlags = CharacterStateFlag.PAUSE_GAUGE | CharacterStateFlag.PAUSE_STATE | CharacterStateFlag.PAUSE_PHYSICS;
        gm.targetGroup.Targets.Clear();

        yield return new WaitForSeconds(0.5f);

        gm.cameraFraming.FovRange = new(90, 90);
        
        // entry cinematic
        var player = gm.CreatePlayer(0, players[0].character.gameObject);
        yield return new WaitForFixedUpdate();
        yield return PlaySingleCharacterEntry(player);
        
        player = gm.CreatePlayer(1, players[1].character.gameObject);
        yield return new WaitForFixedUpdate();
        yield return PlaySingleCharacterEntry(player);
        
        // center camera
        gm.targetGroup.Targets.Clear();
        gm.targetGroup.AddMember(gm.players[0].transform, 1, 0.5f);
        gm.targetGroup.AddMember(gm.players[1].transform, 1, 0.5f);
        gm.cameraFraming.FovRange = new(110, 110);
        
    }

    private IEnumerator BeginNewRoundRoutine() {
        yield return new WaitForSeconds(0.2f);
        
        gm.extraGlobalStateFlags = CharacterStateFlag.PAUSE_GAUGE | CharacterStateFlag.PAUSE_STATE | CharacterStateFlag.PAUSE_PHYSICS;
        gm.targetGroup.Targets.Clear();
        gm.cameraFraming.FovRange = new(110, 110);
        
        // entry cinematic
        gm.CreatePlayer(0, players[0].character.gameObject);
        yield return new WaitForFixedUpdate();
        gm.players[0].OnRoundInit();
        
        gm.CreatePlayer(1, players[1].character.gameObject);
        yield return new WaitForFixedUpdate();
        gm.players[1].OnRoundInit();
        
        // center camera
        gm.targetGroup.Targets.Clear();
        gm.targetGroup.AddMember(gm.players[0].transform, 1, 0.5f);
        gm.targetGroup.AddMember(gm.players[1].transform, 1, 0.5f);
        
        yield return new WaitForFixedUpdate();
        RoundInit();
        yield return new WaitForFixedUpdate();

        
        // apply carried
        foreach (var player in gm.players.Values) {
            player.burst.gauge.value = players[player.playerIndex].carriedData.carriedBurst;
            player.burst.burstAvailable = player.burst.gauge.value >= 450f;
        }
        
        foreach (var comp in Object.FindObjectsByType<RotaryCounter>(FindObjectsInactive.Include, FindObjectsSortMode.None)) {
            comp.ApplyImmediately();
        }
        BattleAnnouncerUI.inst.transitionCoverVisible = false;
        
        yield return RoundStartRoutine();
        unmanaged = true;
        gm.inGame = true;
    }

    private IEnumerator PlaySingleCharacterEntry(PlayerCharacter player) {
        player.OnRoundInit();
        player.rotationContainer.localEulerAngles = new(0, player.side == 0 ? 0 : 180, 0);
        
        gm.targetGroup.Targets.Clear();
        gm.targetGroup.AddMember(player.transform, 1, 0.5f);
        
        player.animation.animation.timeScale = .5f;
        var state = player.animation.animation.state;
        state.ClearTracks();
        state.AddAnimation(0, "defaults/Start", false, 0);
        state.AddAnimation(0, "defaults/Idle", true, 0);
        
        gm.CallLaterCoroutine(0.2f, () => {
            AudioManager.inst.PlayAudioClip("cmn/battle/sfx/deploy", gm.gameObject);
        });

        gm.CallLaterCoroutine(0.5f, () => {
            AudioManager.inst.PlayAudioClip($"chr/{player.descriptor.id}/general/vo/deploy", gm.gameObject, "active_vo");
        });
        
        float timer = 0;
        while (timer <= player.descriptor.entryCinematicDuration) {
            timer += Time.deltaTime;

            foreach (var p in gm.players.Values) {
                p.animation.animation.Update(Time.deltaTime);
            }
            
            yield return null;
        }
    }

    private IEnumerator RoundStartRoutine() {
        yield return new WaitForSeconds(1f);
        
        BattleAnnouncerUI.inst.Show(round);
        AudioManager.inst.PlayAudioClip($"cmn/announcer/battle/round/{round}", gm.gameObject, "active_announcer");
        yield return new WaitForSeconds(1.5f);
        AudioManager.inst.PlayAudioClip("cmn/announcer/battle/fight", gm.gameObject, "active_announcer");
        
        yield return new WaitForSeconds(.5f);
        gm.extraGlobalStateFlags = CharacterStateFlag.NONE;
        Physics2D.SyncTransforms();
    }

    private IEnumerator RoundEndRoutine(PlayerCharacter winner, RoundCompletionStatus status, bool isGameEnd) {
        if (status == RoundCompletionStatus.PERFECT) {
            AudioManager.inst.PlayAudioClip("cmn/announcer/battle/perfect", gm.gameObject, "active_announcer");
        } else if (isGameEnd) {
            AudioManager.inst.PlayAudioClip("cmn/announcer/battle/game", gm.gameObject, "active_announcer");
        } else {
            AudioManager.inst.PlayAudioClip("cmn/announcer/battle/ko", gm.gameObject, "active_announcer");
        }
        yield return new WaitForSeconds(0.3f);
        
        BattleAnnouncerUI.inst.EndRound(status, isGameEnd);
        AudioManager.inst.PlayAudioClip($"cmn/battle/sfx/ui/roundend/{(status == RoundCompletionStatus.PERFECT ? 1 : 0)}", gm.gameObject);
        if (isGameEnd) {
            AudioManager.inst.StopBGM(2.5f);
        }
        
        yield return new WaitForSeconds(2.5f);
        if (!isGameEnd) BattleUIManager.inst.animator.SetBool("Preround", false);
        AudioManager.inst.PlayAudioClip("cmn/battle/sfx/ui/roundend/badge", gm.gameObject);
        UpdateBadges();
        yield return new WaitForSeconds(1f);

        if (!isGameEnd) {
            AudioManager.inst.PlayAudioClip($"chr/{winner.descriptor.id}/general/vo/roundwon", gm.gameObject, "active_vo");
            winner.activeState.stateData.cameraData.cameraFovModifier = -10f;
            yield return new WaitForSeconds(winner.descriptor.roundWonCinematicDuration);
            
        } else {
            yield return new WaitForSeconds(2f);
        }

        // continue
        BattleAnnouncerUI.inst.transitionCoverVisible = true;
        yield return new WaitForSeconds(1f);
        gm.inGame = false;

        if (isGameEnd) {
            yield return new WaitForSeconds(1f);
            AudioManager.inst.PlayBGM("bgm/results/victory", gm.gameObject);
            yield return new WaitForSeconds(.5f);
            
            AudioManager.inst.PlayAudioClip($"chr/{winner.descriptor.id}/general/vo/victory", gm.gameObject, "active_vo");
            
            BattleResultsUI.inst.visible = true;
            PostProcessManager.inst.showBlur = true;
            BattleAnnouncerUI.inst.transitionCoverVisible = false;
            BattleResultsUI.inst.ShowRoundEnd(this, players[winner.playerIndex]);
            
        } else {
            LoadAndBeginNewRound();
        }
    }
    
    protected IEnumerator LoadMatchSceneRoutine() {
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
        var handle = SceneManager.LoadSceneAsync(stageData.sceneId);
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
        AudioManager.inst.PlayBGM(bgmData, RoomManager.inst.gameObject, 1f, .2f);
        yield return new WaitForSeconds(3);
        loadingScreen.coverFadeSpeed = 5f;
        loadingScreen.visible = false;
    }

    //endregion
}

public enum RoomStatus : ushort {
    STANDBY = 0,
    WAIT_ACCEPT = 1,
    CHARACTER_SELECT = 2,
    NEGOTIATING = 3,
    CLIENT_LOADING = 4,
    PLAYING = 5,
    FINISHED = 6
}

public struct RoundResult {
    public Player winner;
    public RoundCompletionStatus completionStatus;
    
    public RoundResult(Player winner, RoundCompletionStatus completionStatus) {
        this.winner = winner;
        this.completionStatus = completionStatus;
    }
}
}
