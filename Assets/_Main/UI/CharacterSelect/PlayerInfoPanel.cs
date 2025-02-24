using System;
using Sirenix.OdinInspector;
using Spine.Unity;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Room;
using SuperSmashRhodes.UI.Generic;
using SuperSmashRhodes.Util;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.CharacterSelect {
public class PlayerInfoPanel : MonoBehaviour {
    public int playerId;
    
    [Title("References")]
    public CanvasGroup infoPanel;
    public CanvasGroup standbyPanel;
    public SkeletonGraphic skeletonGraphic;
    public TMP_Text operatorName;
    public ScrollingText operatorSubname, operatorOrganization;
    public Image emblem;
    public CanvasGroup emblemCarousel;
    
    public CharacterSelectData data => CharacterSelectUI.inst.playerData.TryGetValue(playerId, out var ret) ? ret : null;

    private CharacterDescriptor lastCharacter;
    
    private void Start() {
        emblemCarousel.alpha = 1f;
        emblem.color = Color.clear;
    }

    private void Update() {
        infoPanel.alpha = Mathf.Lerp(infoPanel.alpha, data != null ? 1 : 0, Time.deltaTime * 10);
        standbyPanel.alpha = Mathf.Lerp(standbyPanel.alpha, data == null ? 1 : 0, Time.deltaTime * 10);
        skeletonGraphic.color = Color.Lerp(skeletonGraphic.color, Color.white.ApplyAlpha(data != null ? 1f : 0f), Time.deltaTime * 10f);
        
        if (data == null) return;
        
        // selection
        emblemCarousel.alpha = Mathf.Lerp(emblemCarousel.alpha, data.selectedCharacter ? 0 : 1, Time.deltaTime * 5f);
        emblem.color = Color.Lerp(emblem.color, Color.white.ApplyAlpha(data.selectedCharacter ? 0.1f : 0f), Time.deltaTime * 10f);
        
        if (data.selectedCharacter) {
            var character = data.selectedCharacter;
            if (character != lastCharacter) {
                lastCharacter = character;
                SingleUpdate();
            }
            
            operatorName.text = character.chineseName;
            operatorSubname.text = $"{character.realName} {character.englishName}";
            operatorOrganization.text = $"{character.organizationChinese} {character.organizationEnglish}";

            emblem.transform.localScale = Vector3.Lerp(emblem.transform.localScale, Vector3.one * 1.2f, Time.deltaTime * 10f);
        }
        
    }

    private void SingleUpdate() {
        skeletonGraphic.skeletonDataAsset = lastCharacter.skeletonData;
        skeletonGraphic.Initialize(true);
        skeletonGraphic.AnimationState.SetAnimation(0, "defaults/Idle", true);   
        
        emblem.sprite = lastCharacter.emblem;
        emblem.color = Color.white.ApplyAlpha(0f);
        emblem.transform.localScale = Vector3.one * 0.5f;
    }
}
}
