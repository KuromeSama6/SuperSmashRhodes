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
using SuperSmashRhodes.Network.RoomManagement;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Match.Player;
using SuperSmashRhodes.Scripts.Audio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;

namespace SuperSmashRhodes.UI.CharacterSelect {
public class CharacterSelectUI : SingletonBehaviour<CharacterSelectUI> {
    
    [Title("References")]
    public Animator animator;
    public CanvasGroup controllerWaitPanel;
    public CanvasGroup characterSelectPanel;
    public CanvasGroup playerListPanel;
    public RectTransform drawerContainer;
    public RectTransform portraitContainer;

    [Title("Debug")]
    public RoomConfiguration debugRoomConfiguration;
    public StageData stageData;
    public StageBGMData bgmData;
    
    private bool showPlayerList => room.players.Count >= 1;
    private Room room => RoomManager.current;
    public Dictionary<CharacterDescriptor, CharacterPortrait> portraits = new();
    
    public bool show { get; set; }
    public RoomConfiguration roomConfiguration => room != null ? room.config : null;
    
    private Coroutine showVSScreenRoutine;
    
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
        
        // debug room configuration
        if (debugRoomConfiguration && room == null) {
            // create debug room
            print("Creating debug room");
            var room = new LocalRoom(debugRoomConfiguration);
            RoomManager.inst.CreateRoom(room);
        }
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
        
        if (!show || !roomConfiguration) return;
        
        if (room is LocalRoom localRoom) {
            // player joining
            foreach (var (k, input) in InputDevicePool.inst.inputs) {
                if (room.players.Values.Any(c => c is LocalPlayer localPlayer && localPlayer.input == input)) {
                    continue;
                }
                
                if (input["Submit"].triggered) {
                    var player = localRoom.AddLocalPlayer(k);
                    var character = portraits.Keys.ToList()[player.playerId];
                    
                    player.SetCharacter(character);
                    AudioManager.inst.PlayAudioClip("cmn/sfx/ui/button/normal", gameObject);
                    return;
                }
            }
        }
        
        // charater select
        var keylist = portraits.Keys.ToList();
        
        // input
        if (room.status == RoomStatus.CHARACTER_SELECT) {
            foreach (var (k, player) in room.players.ToList()) {
                if (player is LocalPlayer localPlayer) {
                    var input = localPlayer.input;
                
                    if (input["Navigate"].triggered && player.character && !player.characterConfirmed) {
                        var value = input["Navigate"].ReadValue<Vector2>().x;
                        if (value == 0) continue;
                        var currentIndex = keylist.IndexOf(player.character);
                        if (value > 0) {
                            currentIndex++;
                            if (currentIndex >= portraits.Keys.Count) currentIndex = 0;
                        } else {
                            currentIndex--;
                            if (currentIndex < 0) currentIndex = portraits.Keys.Count - 1;
                        }

                        AudioManager.inst.PlayAudioClip("cmn/ui/navigate/normal", gameObject);
                        localPlayer.SetCharacter(keylist[currentIndex]);
                    }
            
            
                    if (input["Cancel"].triggered) {
                        if (player.characterConfirmed) {
                            localPlayer.SetConfirmed(false);
                        
                        } else if (room is LocalRoom) {
                            room.RemovePlayer(player.playerId);
                        }
                    
                        AudioManager.inst.PlayAudioClip("cmn/sfx/ui/button/back", gameObject);
                    }
                    
                    if (input["Submit"].triggered) {
                        localPlayer.SetConfirmed(true);
                        
                    }   
                }
            }
        }
        
        if (room.allConfirmed && showVSScreenRoutine == null) {
            showVSScreenRoutine = StartCoroutine(ShowVSScreen());
            Debug.Log("beginning match");
            return;
        }
    }

    private IEnumerator ShowVSScreen() {
        if (!show) yield break;
        yield return new WaitForSeconds(2);

        if (!room.allConfirmed) {
            showVSScreenRoutine = null;
            yield break;
        }
        
        CharacterVSScreen.inst.Show();
        show = false;

        yield return new WaitForSeconds(0.5f);
        
        if (room is LocalRoom localRoom) {
            localRoom.BeginMatch(stageData, bgmData);
        } else if (room is NetworkRoom networkRoom) {
            networkRoom.BeginMatchLocal();
        }
    }
}
}
