using System.Collections;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Stage;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Network.Rollbit;
using SuperSmashRhodes.Room;
using SuperSmashRhodes.Scripts.Audio;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.Util;
using UnityEditor.Localization.Plugins.XLIFF.V12;
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
        if (unmanaged && time >= 0 && !config.infiniteTime && !GameManager.inst.globalStateFlags.HasFlag(CharacterStateFlag.GLOBAL_PAUSE_TIMER)) {
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
            for (int i = 0; i < (config.winRounds * 2 - 1); i++) {
                if (i < results.Count) {
                    var result = results[i];
                    badges.Add(result.winner == player ? result.completionStatus : RoundCompletionStatus.LOST);
                    
                } else {
                    badges.Add(RoundCompletionStatus.UNKNOWN);
                }
            }
            
            TopCharacterInfo.Get(player.playerId).SetBadges(badges);
        }
    }
    
    // Routines
    private IEnumerator MainRoutine() {
        yield return GameEntryCinematicRoutine();
        
        // start logic
        RoundInit();
        yield return RoundStartRoutine();
        unmanaged = true;
        GameManager.inst.inGame = true;

    }

    private void RoundInit() {
        ++round;
        GameManager.inst.inGame = false;
        
        BattleUIManager.inst.animator.SetTrigger("Handover");
        BattleUIManager.inst.animator.SetBool("Preround", false);
        gm.extraGlobalStateFlags = CharacterStateFlag.MANAGED;
        time = config.infiniteTime ? 999 : config.roundTime;
        
        BattleTimer.inst.counter.ApplyImmediately();
        
        foreach (var player in GameManager.inst.players.Values) {
            player.BeginLogic();
        }
        
        UpdateBadges();
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
        
        // update for 2 more seconds
        // for (float timer = 0; timer <= 2; timer += Time.deltaTime) {
        //     foreach (var p in gm.players.Values) {
        //         p.animation.animation.Update(Time.deltaTime);
        //     }
        //     yield return null;
        // }
        
        // center camera
        gm.targetGroup.Targets.Clear();
        gm.targetGroup.AddMember(gm.players[0].transform, 1, 0.5f);
        gm.targetGroup.AddMember(gm.players[1].transform, 1, 0.5f);
        gm.cameraFraming.FovRange = new(110, 110);
        
        // put back the ui
        
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
        AudioManager.inst.PlayAudioClip($"cmn/announcer/battle/round/{round}", GameManager.inst.gameObject, "active_announcer");
        yield return new WaitForSeconds(1.5f);
        AudioManager.inst.PlayAudioClip("cmn/announcer/battle/fight", GameManager.inst.gameObject, "active_announcer");
        
        yield return new WaitForSeconds(.5f);
        gm.extraGlobalStateFlags = CharacterStateFlag.NONE;
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
