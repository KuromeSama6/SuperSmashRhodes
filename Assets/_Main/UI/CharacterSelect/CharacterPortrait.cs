using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.UI.Generic;
using SuperSmashRhodes.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.CharacterSelect {
public class CharacterPortrait : MonoBehaviour {
    public CharacterDescriptor character;
    
    [Title("References")]
    public Image avatar;
    public Image avatarBackground;
    public Image emblem;
    public Image professionIcon;
    public TMP_Text operatorName;
    public ScrollingText organizationText, englishText;
    
    private void Start() {
        if (!character) return;
        englishText.text = $"{character.englishName}  {character.realName}".ToUpper();

        
    }

    private void Update() {
        if (!character) return;
        avatar.sprite = avatarBackground.sprite = character.avatar;
        emblem.sprite = character.emblem;
        operatorName.text = character.chineseName;
        professionIcon.sprite = character.professionIconUI;
    
        // englishText.text = $"{character.englishName}  {character.realName}";
        // organizationText.text = $"{character.organizationEnglish}  {character.organizationChinese}";
        
    }

}
}
