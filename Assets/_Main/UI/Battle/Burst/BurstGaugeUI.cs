using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Runtime.State;
using SuperSmashRhodes.UI.Generic;
using SuperSmashRhodes.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Battle.Burst {
public class BurstGaugeUI : PerSideUIElement<BurstGaugeUI> {
    [Title("References")]
    public RectTransform container;
    public RotaryCounter counter;
    public UnityEngine.UI.Outline boxOutline;
    public Image outlineImage;
    public TMP_Text burstText;
    public GameObject burstAvailableTick;
    public Image driveReleaseAvailableIndicator, burstAvailableIndicator, burstDisabledIndicator;
    
    [Title("Config")]
    public Vector2 yRange;

    private float originalX;

    private RectTransform rectTransform => (RectTransform) transform;
    private float offsetX {
        get {
            if (!player) return 0f;
            if (!player.burst.driveRelease && !(player.activeState is State_CmnDriveRelease)) return 0f;

            return player.playerIndex == 0 ? -150 : 150;
        }
    }
    
    private void Start() {
        originalX = rectTransform.anchoredPosition.x;
        rectTransform.anchoredPosition = new Vector2(originalX, rectTransform.anchoredPosition.y);
    }

    private void Update() {
        var player = this.player;
        if (!player) return;
        
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, new Vector2(originalX + offsetX, rectTransform.anchoredPosition.y), Time.deltaTime * 5f);
        
        // if (GameManager.inst.globalStateFlags.HasFlag(CharacterStateFlag.PAUSE_GAUGE)) return;
        
        var burst = player.burst;
        var gauge = burst.gauge;
        
        var y = Mathf.Lerp(yRange.x, yRange.y, gauge.percentage);
        // Debug.Log(y);
        container.anchoredPosition = new Vector2(container.anchoredPosition.x, y);
        counter.target = gauge.value;
        
        // box outline color
        if (gauge.value >= 600f) {
            outlineImage.color = boxOutline.effectColor = "FF00B6".HexToColor();
        } else if (gauge.value >= 500f) {
            outlineImage.color = boxOutline.effectColor = "00b9ff".HexToColor();
            
        } else if (gauge.value <= 100f) {
            outlineImage.color = "FFDA00".HexToColor();
            boxOutline.effectColor = Time.time % 1 > .5f ? Color.black : "FFDA00".HexToColor();
            
        } else if (gauge.value <= 200f) {
            outlineImage.color = Color.white;
            boxOutline.effectColor = "FFDA00".HexToColor();
            
        } else {
            outlineImage.color = boxOutline.effectColor = Color.white;
            if (burst.burstAvailable)
                boxOutline.effectColor = Color.green;
        }

        driveReleaseAvailableIndicator.enabled = player.burst.canDriveRelease;
        burstAvailableTick.SetActive(!burst.burstUsed && !burst.burstAvailable);
        burstAvailableIndicator.enabled = burst.burstAvailable;
        burstDisabledIndicator.enabled = player.burstDisabled;
        burstAvailableIndicator.color = player.burstDisabled ? "B0B0B0".HexToColor() : "4FFF00".HexToColor();
    }
}
}
