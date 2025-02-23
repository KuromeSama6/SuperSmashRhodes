using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Player;
using SuperSmashRhodes.Room;
using UnityEngine;

namespace SuperSmashRhodes.UI.CharacterSelect {
public class CharacterSelectUI : MonoBehaviour {
    [Title("References")]
    public CanvasGroup controllerWaitPanel;
    public CanvasGroup characterSelectPanel;
    public CanvasGroup playerListPanel;
    public RectTransform drawerContainer;

    private readonly Dictionary<LocalPlayer, CharacterSelectData> playerData = new();
    private bool showPlayerList => playerData.Count > 0;
    
    private void Start() {
        
    }

    private void Update() {
        {
            // panels
            controllerWaitPanel.alpha = Mathf.Lerp(controllerWaitPanel.alpha, showPlayerList ? 0 : 1, Time.deltaTime * 10);
            playerListPanel.alpha = Mathf.Lerp(playerListPanel.alpha, showPlayerList ? 1 : 0, Time.deltaTime * 10);

            var delta = drawerContainer.sizeDelta;
            delta.y = Mathf.Lerp(delta.y, showPlayerList ? 300 : 100, Time.deltaTime * 10);
            drawerContainer.sizeDelta = delta;
        }
    }
}
}
