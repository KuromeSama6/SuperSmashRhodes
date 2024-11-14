using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.UI.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Battle {
public class HealthBar : PerSideUIElement<HealthBar> {
    [Title("References")]
    public Image barFill;
    public Image barCounterFill;
    public Image changeIndicator;
    public RotaryCounter counter;
    
    private float current = 440;

    private void Update() {
        var player = this.player;
        if (player == null) return;

        var actualHealth = player.health;
        var maxHealth = player.config.health;
        var percent = actualHealth / maxHealth;
        
        // set actual health
        barFill.fillAmount = percent;
        
        // set counter
        counter.target = (percent) * 1000f;
        
        barCounterFill.fillAmount = counter.current / 1000f;
        
        // set change indicator
        var step = Time.deltaTime * 100;
        if (actualHealth < current && Mathf.Abs(current - actualHealth) > step) {
            if (player.comboCounter.count == 0) {
                current -= step;
                
            }

        } else current = actualHealth;
        current = Mathf.Max(current, actualHealth);

        changeIndicator.fillAmount = current / maxHealth;

    }

}
}
