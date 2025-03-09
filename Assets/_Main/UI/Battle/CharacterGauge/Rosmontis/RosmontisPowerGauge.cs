using Sirenix.OdinInspector;
using SuperSmashRhodes.Runtime.Gauge;
using SuperSmashRhodes.UI.Generic;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Battle.Rosmontis {
public class RosmontisPowerGauge : ComponentSpecificUIElement<Gauge_Rosmontis_SwordManager> {
    [Title("Preferences")]
    public RotaryCounter counter;
    public Image barFill, barSubfill;
    public Image barOutline;
    public Image apIcon;
    public RectTransform limitFill;

    protected override void Update() {
        base.Update();
        if (!playerComponent) return;

        var gauge = playerComponent.power;

        barSubfill.fillAmount = gauge.percentage;

        var fillTarget = Mathf.Floor(gauge.value) / 5f;
        counter.target = Mathf.Floor(gauge.value);
        barFill.fillAmount = Mathf.Lerp(barFill.fillAmount, fillTarget, Time.deltaTime * 20f);
        // barSubfill.color = (subfillTarget > barSubfill.fillAmount ? Color.white : "FFCA00".HexToColor()).ApplyAlpha(0.2f);
        
        limitFill.anchorMin = Vector2.Lerp(limitFill.anchorMin, new((playerComponent.maxPower) / 5f, 0), Time.deltaTime * 15f);
        
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) {
            playerComponent.UsePower(2);
        }
    }

}
}
