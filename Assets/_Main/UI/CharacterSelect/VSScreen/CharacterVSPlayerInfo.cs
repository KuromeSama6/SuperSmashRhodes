using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Match.Player;
using SuperSmashRhodes.Network.RoomManagement;
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

    public Player data => RoomManager.current.GetPlayer(playerId);

    private void Start() {
        
    }

    private void Update() {
        if (data == null) return;
        var descriptor = data.character;
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
