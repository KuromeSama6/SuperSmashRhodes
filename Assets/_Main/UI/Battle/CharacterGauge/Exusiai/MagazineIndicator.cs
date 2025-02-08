using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Runtime.Gauge;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Battle.Exusiai {
public class MagazineIndicator : MonoBehaviour {
    [Title("References")]
    public Image bg;
    public Image fill;
    
    public Gauge_Exusiai_AmmoGauge gauge { get; set; }
    public Gauge_Exusiai_AmmoGauge.Magazine magazine { get; set; }

    private void Start() {
        fill.fillAmount = 0;
    }

    private void Update() {
        if (magazine == null || !gauge) return;
        
        fill.fillAmount = magazine.ammo / 30f;
        
        var isCurrent = magazine == gauge.currentMagazine;
        fill.color = isCurrent ? "FFDA00".HexToColor() : Color.white;
        bg.color = magazine.ammo == 0 ? "9F3A3A".HexToColor() : "A1A1A1".HexToColor();
    }
}
}
