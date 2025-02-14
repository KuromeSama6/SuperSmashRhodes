using System;
using System.Collections.Generic;
using LeTai.TrueShadow;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Battle.AnnouncerHud {
public class AnnouncerHud : PerSideUIElement<AnnouncerHud> {
    [Title("References")]
    public CanvasGroup canvasGroup;
    public UDictionary<StateIndicatorFlag, Image> stateIndicators;

    private float timeWithoutChange = 0;
    private StateIndicatorFlag state = StateIndicatorFlag.NONE;
    private readonly Dictionary<StateIndicatorFlag, Color> initialColors = new();
    
    private void Start() {
        canvasGroup.alpha = 0;
        timeWithoutChange = 5;
        
        foreach (var entry in stateIndicators) {
            initialColors[entry.Key] = entry.Value.color;
        }
    }

    private void Update() {
        if (!player || player.activeState == null) return;
        var currentState = ((CharacterState)player.activeState).stateIndicator;

        if (currentState != state) {
            timeWithoutChange = 0;
            foreach (var entry in stateIndicators) {
                entry.Value.color = currentState.HasFlag(entry.Key) ? initialColors[entry.Key] : Color.gray;
                entry.Value.GetComponent<TrueShadow>().enabled = currentState.HasFlag(entry.Key);
            }

        } else {
            timeWithoutChange += Time.deltaTime;
        }
        
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, timeWithoutChange > 3f ? 0 : 1, Time.deltaTime * 10);
    }

}

[Flags]
public enum StateIndicatorFlag {
    NONE = 0,
    PUNISH = 1 << 0,
    COUNTER = 1 << 1,
    THROW = 1 << 2,
    THROW_TECH = 1 << 3,
    INVINCIBLE = 1 << 4,
    REVERSAL = 1 << 5,
    SUPER = 1 << 6
}
}
