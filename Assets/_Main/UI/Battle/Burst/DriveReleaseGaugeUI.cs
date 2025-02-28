using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Runtime.State;
using SuperSmashRhodes.UI.Generic;
using SuperSmashRhodes.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Battle.Burst {
public class DriveReleaseGaugeUI : PerSideUIElement<DriveReleaseGaugeUI> {
    [Title("References")]
    public Image fill;
    public RotaryCounter counter;

    private float originalX;

    private RectTransform rectTransform => (RectTransform) transform;
    private float offsetX {
        get {
            if (!player || !player.logicStarted) return player.playerIndex == 0 ? -150 : 150;
            if (!player.burst.driveRelease) return player.playerIndex == 0 ? -150 : 150;
            if (player.activeState.type.HasFlag(EntityStateType.CHR_ATK_SUPER)) return player.playerIndex == 0 ? -150 : 150;

            return 0;
        }
    }
    
    private void Start() {
        originalX = rectTransform.anchoredPosition.x;
        rectTransform.anchoredPosition = new Vector2(originalX, rectTransform.anchoredPosition.y);
    }

    private void Update() {
        var player = this.player;
        if (player == null || !player.burst) return;
        
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, new Vector2(originalX + offsetX, rectTransform.anchoredPosition.y), Time.deltaTime * 10f);
        
        // if (GameManager.inst.globalStateFlags.HasFlag(CharacterStateFlag.PAUSE_GAUGE)) return;

        var time = player.burst.releaseFrames;
        fill.fillAmount = time / player.burst.maxReleaseFrames;
        // Debug.Log(time / 60f * 10f);
        counter.target = Mathf.Max(0, time / 60f * 10f);
    }
}
}
