using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Util;
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
    public RectTransform badgeContainer;
    public GameObject badgeImagePrefab;
    public UDictionary<RoundCompletionStatus, Sprite> badgeSprites = new();
    
    private void Start() {
        badgeContainer.gameObject.ClearChildren();
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

    public void SetBadges(List<RoundCompletionStatus> badges) {
        badgeContainer.gameObject.ClearChildren();
        foreach (var badge in badges) {
            var badgeImage = Instantiate(badgeImagePrefab, badgeContainer).GetComponent<Image>();
            badgeImage.sprite = badgeSprites[badge == RoundCompletionStatus.LOST ? RoundCompletionStatus.UNKNOWN : badge];
        }
    }
}
}
