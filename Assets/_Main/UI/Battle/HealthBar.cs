using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.UI.Generic;
using SuperSmashRhodes.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Battle {
public class HealthBar : PerSideUIElement<HealthBar> {
    [Title("References")]
    public Image barFill;
    public Image barCounterFill;
    public Image changeIndicator;
    public RotaryCounter counter;
    public Image portrait;
    
    private float current = 440;
    private Color targetColor = Color.white;

    private void Start() {
        InvokeRepeating("HealthBlink", 0f, .5f);
    }

    private void Update() {
        var player = this.player;
        if (player == null) return;

        var actualHealth = player.health;
        var maxHealth = player.config.health;
        var percent = actualHealth / maxHealth;
        
        // update target color
        targetColor = Color.Lerp(targetColor, Color.white, Time.deltaTime * 7f);
        
        // set actual health
        barFill.fillAmount = percent;
        
        // set counter
        counter.target = (percent) * 100f;
        
        barCounterFill.fillAmount = counter.current / 100f;
        
        // set change indicator
        var step = Time.deltaTime * 100;
        if (actualHealth < current && Mathf.Abs(current - actualHealth) > step) {
            if (player.comboCounter != null && player.comboCounter.displayedCount == 0) {
                current -= step;
                
            }

        } else current = actualHealth;
        current = Mathf.Max(current, actualHealth);

        changeIndicator.fillAmount = current / maxHealth;

        barFill.color = targetColor;
        foreach (var comp in counter.GetComponentsInChildren<TMP_Text>()) {
            comp.color = targetColor.ApplyAlpha(comp.color.a);
        }
        portrait.color = targetColor;
    }

    private void HealthBlink() {
        if (!player) return;
        var actualHealth = player.health;
        var maxHealth = player.config.health;
        var percent = actualHealth / maxHealth;

        if (percent <= .25f) targetColor = "cb0000".HexToColor();
    }

}
}
