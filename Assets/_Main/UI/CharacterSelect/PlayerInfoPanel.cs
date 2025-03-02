using System;
using Sirenix.OdinInspector;
using Spine.Unity;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Match.Player;
using SuperSmashRhodes.Network.RoomManagement;
using SuperSmashRhodes.Scripts.Audio;
using SuperSmashRhodes.UI.Generic;
using SuperSmashRhodes.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.CharacterSelect {
public class PlayerInfoPanel : MonoBehaviour {
    private static readonly int GRAYSCALE = Shader.PropertyToID("_GrayscaleAmount");
    public int playerId;
    
    [Title("References")]
    public CanvasGroup infoPanel;
    public CanvasGroup standbyPanel;
    public SkeletonGraphic skeletonGraphic;
    public TMP_Text operatorName;
    public ScrollingText operatorSubname, operatorOrganization;
    public Image emblem;
    public CanvasGroup emblemCarousel;
    public Image portrait;
    public RectTransform cornerScroll;
    public CanvasGroup namesPanel;
    public CanvasGroup readyBadge;

    public Player player => RoomManager.current.GetPlayer(playerId);

    private CharacterDescriptor lastCharacter;
    private bool lastConfirmState;
    
    private void Start() {
        emblemCarousel.alpha = 1f;
        emblem.color = Color.clear;

        portrait.material = new(portrait.material);
    }

    private void Update() {
        infoPanel.alpha = Mathf.Lerp(infoPanel.alpha, player != null ? 1 : 0, Time.deltaTime * 10);
        standbyPanel.alpha = Mathf.Lerp(standbyPanel.alpha, player == null ? 1 : 0, Time.deltaTime * 10);
        skeletonGraphic.color = Color.Lerp(skeletonGraphic.color, Color.white.ApplyAlpha(player != null ? 1f : 0f), Time.deltaTime * 10f);
        portrait.color = Color.Lerp(portrait.color, player != null && player.character ? (player.characterConfirmed ? Color.white : Color.Lerp(Color.black, Color.white, .8f)) : Color.clear, Time.deltaTime * 10f);
        emblemCarousel.alpha = Mathf.Lerp(emblemCarousel.alpha, player != null && player.character ? 0 : 1, Time.deltaTime * 5f);
        emblem.color = Color.Lerp(emblem.color, Color.white.ApplyAlpha(player != null && player.character ? (player.characterConfirmed ? 0.5f : 0.1f) : 0f), Time.deltaTime * 10f);
        
        cornerScroll.sizeDelta = new Vector2(cornerScroll.sizeDelta.x, Mathf.Lerp(cornerScroll.sizeDelta.y, player != null && player.characterConfirmed ? 0f : 30f, Time.deltaTime * 15f));
        LayoutRebuilder.ForceRebuildLayoutImmediate(cornerScroll.transform.parent as RectTransform);

        {
            var ready = player != null && player.characterConfirmed;
            readyBadge.alpha = Mathf.Lerp(readyBadge.alpha, ready ? 1f : 0f, Time.deltaTime * 30f);
            readyBadge.transform.localScale = Vector3.Lerp(readyBadge.transform.localScale, Vector3.one * (ready ? 1f : 1.5f), Time.deltaTime * 20f);

            operatorSubname.scrollSpeed = operatorOrganization.scrollSpeed = ready ? 200f : 50f;
            portrait.material.SetFloat(GRAYSCALE, ready ? 0f : .5f);
        }
        
        if (player == null) {
            lastCharacter = null;
            return;
        }
        
        // selection
        
        if (player.character) {
            var character = player.character;
            if (character != lastCharacter) {
                lastCharacter = character;
                SingleUpdate();
            }
            
            operatorName.text = character.chineseName;
            operatorSubname.text = $"{character.realName} {character.englishName}";
            operatorOrganization.text = $"{character.organizationChinese} {character.organizationEnglish}";
            
            emblem.transform.localScale = Vector3.Lerp(emblem.transform.localScale, Vector3.one * (player.characterConfirmed ? .9f : 1.2f), Time.deltaTime * 10f);
            
            namesPanel.alpha = Mathf.Lerp(namesPanel.alpha, player.characterConfirmed ? 1f : 0.5f, Time.deltaTime * 10f);
            
            if (player.characterConfirmed != lastConfirmState) {
                lastConfirmState = player.characterConfirmed;
                ConfirmUpdate(player.characterConfirmed);
            }

            var comp = portrait.GetComponent<RectTransform>();
            comp.anchoredPosition = Vector2.Lerp(comp.anchoredPosition, Vector2.zero, Time.deltaTime * 10f);
        }
        
    }

    private void SingleUpdate() {
        skeletonGraphic.skeletonDataAsset = lastCharacter.skeletonData;
        skeletonGraphic.Initialize(true);
        skeletonGraphic.AnimationState.SetAnimation(0, "defaults/Relax", true);   
        
        emblem.sprite = lastCharacter.emblem;
        emblem.color = Color.white.ApplyAlpha(0f);
        emblem.transform.localScale = Vector3.one * 0.5f;
        
        portrait.sprite = lastCharacter.portrait;
        portrait.GetComponent<RectTransform>().anchoredPosition -= new Vector2(20f, 0f);
        portrait.color = Color.clear;
    }

    private void ConfirmUpdate(bool confirmed) {
        var animation = skeletonGraphic.AnimationState;
        if (confirmed) {
            var owner = CharacterSelectUI.inst.gameObject;
            AudioManager.inst.PlayAudioClip("cmn/sfx/ui/button/heavy", owner);
            AudioManager.inst.PlayAudioClip($"chr/{player.character.id}/general/vo/entry", owner, "active_vo");
            this.CallLaterCoroutine(0.2f, () => {
                AudioManager.inst.PlayAudioClip("cmn/battle/sfx/deploy", owner);
            });
            
            animation.SetAnimation(0, "defaults/Start", false);
            animation.AddAnimation(0, "defaults/Idle", true, 0);
            animation.GetCurrent(0).MixDuration = 0;
            
            AudioManager.inst.PlayAudioClip("cmn/sfx/ui/button/heavy", owner);
            
        } else {
            animation.SetAnimation(0, "defaults/Relax", true);
            animation.GetCurrent(0).MixDuration = 0.1f;
        }
    }
    
}
}
