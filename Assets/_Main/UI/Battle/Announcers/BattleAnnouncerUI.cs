using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Framework;
using TMPro;
using UnityEngine;

namespace SuperSmashRhodes.UI.Battle {
public class BattleAnnouncerUI : SingletonBehaviour<BattleAnnouncerUI> {
    [Title("References")]
    public Animator roundStartBannerAnimator;
    public TMP_Text roundText;

    private void Start() {
        
    }

    private void Update() {
        
    }

    public void Show(int roundNumber) {
        roundStartBannerAnimator.SetTrigger("Show");
        roundText.text = $"Round {roundNumber}";
    }
}
}
