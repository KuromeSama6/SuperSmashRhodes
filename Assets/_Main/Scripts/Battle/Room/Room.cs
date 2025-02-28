using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Stage;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Network.Rollbit;
using SuperSmashRhodes.Room;
using SuperSmashRhodes.Scripts.Audio;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using SuperSmashRhodes.UI.Generic;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

namespace SuperSmashRhodes.Network.Room {
public abstract class Room {
    public RoomConfiguration config { get; private set; }
    public int playerCount { get; protected set; } = 0;
    public int spectatorCount { get; protected set; } = 0;
    public StageData stageData { get; set; }
    public StageBGMData bgmData { get; set; }
    
    public UnityEvent onDestroy { get; } = new();
    public UnityEvent<RoomStatus> onStatusChange { get; } = new();
    public Dictionary<int, PlayerMatchData> players { get; } = new();
    public float time { get; protected set; } = 999;
    public int round { get; protected set; }
    public List<RoundResult> results { get; } = new();

    public RoomStatus status {
        get => _status;
        protected set {
            if (value != status) {
                onStatusChange.Invoke(value);
            }
            _status = value;
        }
    }
    
    private RoomStatus _status = RoomStatus.STANDBY;
    private bool unmanaged = false;
    
    private GameManager gm => GameManager.inst;
    
    public Room(RoomConfiguration config) {
        this.config = config;
    }

    public void Destroy() {
        onDestroy.Invoke();
        onDestroy.RemoveAllListeners();
    }

    public void BeginMatch() {
        RoomManager.inst.StartCoroutine(MainRoutine());
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
        
        RoomManager.inst.StartCoroutine(RoundEndRoutine(winner, status, isGameEnd));
        BattleUIManager.inst.animator.SetBool("Preround", true);
        
        foreach (var player in gm.players.Values) {
            SaveCarriedData(player);
        }
    }

    private void LoadAndBeginNewRound() {
        GameManager.inst.ResetRound();
        
        RoomManager.inst.StartCoroutine(BeginNewRoundRoutine());
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
    
    // Routines
    private IEnumerator MainRoutine() {
        yield return GameEntryCinematicRoutine();
        
        // start logic
        RoundInit();
        yield return RoundStartRoutine();
        unmanaged = true;
        gm.inGame = true;
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
        var player = gm.CreatePlayer(0, players[0].selectedCharacter.gameObject);
        yield return new WaitForFixedUpdate();
        yield return PlaySingleCharacterEntry(player);
        
        player = gm.CreatePlayer(1, players[1].selectedCharacter.gameObject);
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
        gm.CreatePlayer(0, players[0].selectedCharacter.gameObject);
        yield return new WaitForFixedUpdate();
        gm.players[0].OnRoundInit();
        
        gm.CreatePlayer(1, players[1].selectedCharacter.gameObject);
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
        
        if (!isGameEnd) LoadAndBeginNewRound();
    }

    private void SaveCarriedData(PlayerCharacter player) {
        var data = players[player.playerIndex];
        data.carriedData.carriedBurst = player.burst.gauge.value;
    }
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
    public PlayerMatchData winner;
    public RoundCompletionStatus completionStatus;
    
    public RoundResult(PlayerMatchData winner, RoundCompletionStatus completionStatus) {
        this.winner = winner;
        this.completionStatus = completionStatus;
    }
}
}
