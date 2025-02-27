using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Room;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.CharacterSelect {
public class CharacterVSPlayerInfo : MonoBehaviour {
    public int playerId;

    [Title("References")]
    public Image portrait;
    public Image portraitBackground;
    public Image emblem;
    public Image professionIcon;
    public Image easeOfUseIcon;
    public TMP_Text name, englishName, mainDescription, subDescription;

    public PlayerMatchData data => CharacterSelectUI.inst.playerData.GetValueOrDefault(playerId);

    private void Start() {
        
    }

    private void Update() {
        if (data == null) return;
        var descriptor = data.selectedCharacter;
        if (!descriptor) return;
        
        portrait.sprite = portraitBackground.sprite = descriptor.portrait;
        emblem.sprite = descriptor.emblem;
        professionIcon.sprite = descriptor.professionIconBlack;
        easeOfUseIcon.sprite = descriptor.easyOfUseRatingIcon;
        
        name.text = descriptor.chineseName;
        englishName.text = descriptor.englishName;
        mainDescription.text = descriptor.mainDescription;
        subDescription.text = $"{descriptor.subDescription1}\n{descriptor.subDescription2}";

    }

}
}
