using System;
using System.Linq;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Match.Player;
using SuperSmashRhodes.Network.RoomManagement;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.CharacterSelect {
public class CharacterSelectCursor : MonoBehaviour {
    public int playerId;

    [Title("References")]
    public CanvasGroup canvasGroup;
    public Image cursorOutline, cursorFill;
    public CanvasGroup p1Pointer, p2Pointer;

    private Player data => RoomManager.current.GetPlayer(playerId);
    
    private void Start() {
        canvasGroup.alpha = 0;
    }

    private void Update() {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, data != null ? 1f : 0f, Time.deltaTime * 10f);
        if (data == null) return;
        var hasMultipleSelected = RoomManager.current.players.Values.Count(c => c.character == data.character) > 1;
        
        {
            // cursor color
            Color cursorColor;
            if (hasMultipleSelected) {
                cursorColor = Color.white;
            } else {
                cursorColor = playerId == 0 ? "00b9ff".HexToColor() : "FF6BE5".HexToColor();
            }
            
            cursorOutline.color = Color.Lerp(cursorOutline.color, cursorColor, Time.deltaTime * 10);
            cursorFill.color = Color.Lerp(cursorFill.color, cursorColor.ApplyAlpha(hasMultipleSelected && playerId != 0 ? 0f : 0.1f), Time.deltaTime * 10);
        }

        {
            // pointers
            p1Pointer.alpha = Mathf.Lerp(p1Pointer.alpha, playerId == 0 ? 1 : 0, Time.deltaTime * 10);
            p2Pointer.alpha = Mathf.Lerp(p2Pointer.alpha, playerId == 1 ? 1 : 0, Time.deltaTime * 10);
        }

        // update portrait location
        var portrait = CharacterSelectUI.inst.portraits[data.character];
        var rect = transform as RectTransform;
        var pos = rect.anchoredPosition;
        pos.x = Mathf.Lerp(pos.x, ((RectTransform)portrait.transform).anchoredPosition.x, Time.deltaTime * 15);
        rect.anchoredPosition = pos;

        cursorOutline.transform.localScale = Vector3.Lerp(cursorOutline.transform.localScale, Vector3.one * (data.characterConfirmed ? .95f : 1f), Time.deltaTime * 30f);

    }
}
}
