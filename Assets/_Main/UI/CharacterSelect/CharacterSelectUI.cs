using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Player;
using SuperSmashRhodes.Room;
using UnityEngine;
using UnityEngine.InputSystem.UI;

namespace SuperSmashRhodes.UI.CharacterSelect {
public class CharacterSelectUI : SingletonBehaviour<CharacterSelectUI> {
    [Title("References")]
    public CanvasGroup controllerWaitPanel;
    public CanvasGroup characterSelectPanel;
    public CanvasGroup playerListPanel;
    public RectTransform drawerContainer;
    public RectTransform portraitContainer;

    public Dictionary<int, CharacterSelectData> playerData { get; } = new();
    private bool showPlayerList => playerData.Count > 0;
    public Dictionary<CharacterDescriptor, CharacterPortrait> portraits = new();
    
    private void Start() {
        InputDevicePool.inst.currentActionMap = "UI";
        
        // load portraits
        foreach (RectTransform rect in portraitContainer.transform) {
            var portrait = rect.GetComponent<CharacterPortrait>();
            if (!portrait || !portrait.character) continue;
            portraits[portrait.character] = portrait;
        }
    }

    private void Update() {
        {
            // player joining
            foreach (var (k, input) in InputDevicePool.inst.inputs) {
                if (playerData.Values.Any(c => c.input == input)) {
                    if (input["Cancel"].triggered) {
                        var playerId = playerData.First(c => c.Value.input == input).Key;
                        playerData.Remove(playerId);
                    }
                    continue;
                }
                
                if (input["Submit"].triggered) {
                    int playerId = playerData.ContainsKey(0) ? 1 : 0;
                    
                    var data = new CharacterSelectData(input);
                    data.selectedCharacter = portraits.Keys.ToList()[playerId];
                    
                    playerData[playerId] = data;
                }
            }
        }
        
        // charater select
        var keylist = portraits.Keys.ToList();
        foreach (var (k, player) in playerData) {
            var input = player.input;
            if (input["Navigate"].triggered) {
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
                
                player.selectedCharacter = keylist[currentIndex];
            }
        }
        
        {
            // panels
            controllerWaitPanel.alpha = Mathf.Lerp(controllerWaitPanel.alpha, showPlayerList ? 0 : 1, Time.deltaTime * 10);
            playerListPanel.alpha = Mathf.Lerp(playerListPanel.alpha, showPlayerList ? 1 : 0, Time.deltaTime * 10);

            var delta = drawerContainer.sizeDelta;
            delta.y = Mathf.Lerp(delta.y, showPlayerList ? 300 : 100, Time.deltaTime * 15f);
            drawerContainer.sizeDelta = delta;
        }
        
    }
}
}
