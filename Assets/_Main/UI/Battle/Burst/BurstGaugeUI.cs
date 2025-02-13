using System;
using Sirenix.OdinInspector;
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
    public Image burstAvailableIndicator, burstDisabledIndicator;
    
    [Title("Config")]
    public Vector2 yRange;

    private void Update() {
        var player = this.player;
        if (player == null) return;
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
        
        burstAvailableTick.SetActive(!burst.burstUsed && !burst.burstAvailable);
        burstAvailableIndicator.enabled = burst.burstAvailable;
        burstDisabledIndicator.enabled = player.burstDisabled;
        burstAvailableIndicator.color = player.burstDisabled ? "B0B0B0".HexToColor() : "4FFF00".HexToColor();
    }
}
}
