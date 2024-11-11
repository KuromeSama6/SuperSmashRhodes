using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Battle {
public class TopCharacterInfo : PerSideUIElement<TopCharacterInfo> {
    [Title("References")]
    public Image portrait;
    public Image portraitBackground;
    public Image professionIcon;
    public TMP_Text chineseName, englishName;
    
    private void Start() {
        
    }

    private void Update() {
        if (player == null) return;
        var descriptor = player.descriptor;

        portrait.sprite = descriptor.avatar;
        portraitBackground.sprite = descriptor.avatar;
        professionIcon.sprite = descriptor.professionIconUI;
        chineseName.text = descriptor.chineseName;
        englishName.text = descriptor.englishName;
    }
}
}
