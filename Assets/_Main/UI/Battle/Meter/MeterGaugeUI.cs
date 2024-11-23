using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Character.Gauge;
using SuperSmashRhodes.UI.Generic;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Battle {
public class MeterGaugeUI : PerSideUIElement<MeterGaugeUI> {
    [Title("References")]
    public Image barFill;
    public Image barStripes;
    public Image barFillMain;
    public Image changeIndicator, changeIndicatorMask;
    public GameObject halfMeterIndicator, fullMeterIndicator;
    public RotaryCounter counter;

    private float _current;
    
    private void Start() {
        barFill.fillAmount = 0;
        barStripes.color = Color.clear;
        halfMeterIndicator.SetActive(false);
        fullMeterIndicator.SetActive(false);
    }

    private void Update() {
        if (!player) return;
        var gauge = player.meter;
        var meter = gauge.gauge.value;
        var percentage = meter / gauge.gauge.max;

        _current = Mathf.Lerp(_current, percentage, Time.deltaTime * 2f);
        if (percentage > _current) {
            barFill.fillAmount = _current;
            changeIndicator.fillAmount = percentage;
            changeIndicatorMask.fillAmount = 1 - _current;
            changeIndicator.color = "00b9ff".HexToColor().ApplyAlpha(0.6f);
        } else {
            barFill.fillAmount = percentage;
            changeIndicator.fillAmount = _current;
            changeIndicatorMask.fillAmount = 1 - percentage;
            changeIndicator.color = "FF6BE5".HexToColor().ApplyAlpha(0.6f);
        }
        
        barStripes.color = percentage >= 1f ? Color.white : Color.clear;
        halfMeterIndicator.SetActive(percentage >= 0.5f);
        fullMeterIndicator.SetActive(percentage >= 1f);
        counter.target = meter;
        
        if (percentage >= 1f) barFillMain.color = Color.white.ApplyAlpha(.75f);
        else if (percentage >= .5f) barFillMain.color = Color.white.ApplyAlpha(.5f);
        else barFillMain.color = Color.white.ApplyAlpha(.25f);

    }
}
}
