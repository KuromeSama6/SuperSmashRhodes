using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Battle {
public class PortraitCutscenePlayer : PerSideUIElement<PortraitCutscenePlayer> {
    private static readonly int SHOW = Animator.StringToHash("Show");
    [Title("References")]
    public Animator animator;
    public RectTransform portraitContainer;
    public Image portrait, silhouette;

    public bool isPlaying => currentPortrait != null;
    private Sprite currentPortrait;
    private float timeRemaining;
    private Vector2 origialPosition;

    private void Start() {
        origialPosition = portraitContainer.anchoredPosition;
    }

    private void Update() {
        if (currentPortrait != null) {
            timeRemaining -= Time.deltaTime;
            
            if (timeRemaining <= 0) {
                currentPortrait = null;
            }
            
        }
        
        portraitContainer.anchoredPosition -= new Vector2(15f, 25f) * Time.deltaTime * 0.5f;
        animator.SetBool(SHOW, currentPortrait != null);
    }

    public void Play(Sprite sprite, float duration) {
        if (!sprite) return;
        portraitContainer.anchoredPosition = origialPosition;
        timeRemaining = duration;
        currentPortrait = sprite;
        portrait.sprite = sprite;
        silhouette.sprite = sprite;
    }
    
}
}
