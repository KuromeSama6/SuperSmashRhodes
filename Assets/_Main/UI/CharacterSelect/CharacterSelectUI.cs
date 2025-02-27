using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Stage;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Network.Room;
using SuperSmashRhodes.Room;
using SuperSmashRhodes.Scripts.Audio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;

namespace SuperSmashRhodes.UI.CharacterSelect {
public class CharacterSelectUI : SingletonBehaviour<CharacterSelectUI> {
    public RoomConfiguration roomConfiguration;
    
    [Title("References")]
    public Animator animator;
    public CanvasGroup controllerWaitPanel;
    public CanvasGroup characterSelectPanel;
    public CanvasGroup playerListPanel;
    public RectTransform drawerContainer;
    public RectTransform portraitContainer;

    [Title("Debug")]
    public StageData stageData;
    public StageBGMData bgmData;
    
    public Dictionary<int, PlayerMatchData> playerData { get; } = new();
    private bool showPlayerList => playerData.Count > 0;
    public Dictionary<CharacterDescriptor, CharacterPortrait> portraits = new();
    
    public bool show { get; set; }
    
    private void Start() {
        show = true;
        InputDevicePool.inst.currentActionMap = "UI";
        
        // load portraits
        foreach (RectTransform rect in portraitContainer.transform) {
            var portrait = rect.GetComponent<CharacterPortrait>();
            if (!portrait || !portrait.character) continue;
            portraits[portrait.character] = portrait;
        }

        AudioManager.inst.PlayBGM("bgm/characterselect", gameObject, .5f, .3f);
    }

    private void Update() {
        {
            // panels
            controllerWaitPanel.alpha = Mathf.Lerp(controllerWaitPanel.alpha, showPlayerList ? 0 : 1, Time.deltaTime * 10);
            playerListPanel.alpha = Mathf.Lerp(playerListPanel.alpha, showPlayerList ? 1 : 0, Time.deltaTime * 10);

            var delta = drawerContainer.sizeDelta;
            delta.y = Mathf.Lerp(delta.y, show ? showPlayerList ? 300 : 100 : 0, Time.deltaTime * 15f);
            drawerContainer.sizeDelta = delta;
            
            animator.SetBool("Show", show);
        }
        
        if (!show) return;
        
        {
            // player joining
            foreach (var (k, input) in InputDevicePool.inst.inputs) {
                if (playerData.Values.Any(c => c.input == input)) {
                    if (input["Cancel"].triggered) {
                        var playerId = playerData.First(c => c.Value.input == input).Key;
                        var data = playerData[playerId];
                        
                        if (data.confirmed) {
                            data.confirmed = false;
                        } else {
                            playerData.Remove(playerId);
                        }
                        AudioManager.inst.PlayAudioClip("cmn/sfx/ui/button/back", gameObject);
                    }
                    
                    if (input["Submit"].triggered) {
                        var playerId = playerData.First(c => c.Value.input == input).Key;
                        playerData[playerId].confirmed = true;

                        if (playerData.Count >= 2 && playerData.Values.All(c => c.confirmed)) {
                            StopAllCoroutines();
                            StartCoroutine(ShowVSScreen(playerData[0], playerData[1]));
                            return;
                        }
                    }
                    
                    continue;
                }
                
                if (input["Submit"].triggered) {
                    int playerId = playerData.ContainsKey(0) ? 1 : 0;
                    
                    var data = new PlayerMatchData(playerId, input);
                    data.selectedCharacter = portraits.Keys.ToList()[playerId];
                    
                    playerData[playerId] = data;
                    AudioManager.inst.PlayAudioClip("cmn/sfx/ui/button/normal", gameObject);
                }
            }
        }
        
        // charater select
        var keylist = portraits.Keys.ToList();
        foreach (var (k, player) in playerData) {
            var input = player.input;
            if (input["Navigate"].triggered && player.selectedCharacter && !player.confirmed) {
                var value = input["Navigate"].ReadValue<Vector2>().x;
                if (value == 0) continue;
                var currentIndex = keylist.IndexOf(player.selectedCharacter);
                if (value > 0) {
                    currentIndex++;
                    if (currentIndex >= portraits.Keys.Count) currentIndex = 0;
                } else {
                    currentIndex--;
                    if (currentIndex < 0) currentIndex = portraits.Keys.Count - 1;
                }

                AudioManager.inst.PlayAudioClip("cmn/ui/navigate/normal", gameObject);
                player.selectedCharacter = keylist[currentIndex];
            }
        }
    }

    private IEnumerator ShowVSScreen(PlayerMatchData p1, PlayerMatchData p2) {
        if (!show) yield break;
        yield return new WaitForSeconds(2);
        
        if (playerData.Count < 2 || playerData.Values.Any(c => !c.confirmed)) yield break;
        
        CharacterVSScreen.inst.Show(p1, p2);
        show = false;

        yield return new WaitForSeconds(0.5f);
        RoomManager.inst.BeginLocalMatch(roomConfiguration, stageData, bgmData, p1, p2);
        
    }
}
}
