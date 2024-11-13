using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using NUnit.Framework;
using Sirenix.OdinInspector;
using SuperSmashRhodes.UI.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI {
public class ComboCounterUI : PerSideUIElement<ComboCounterUI> {
    [Title("References")]
    public Image mainMask;
    public RotaryCounter counter;
    public Image endMask;
    public CanvasGroup canvasGroup;

    private bool shown = false;
    private List<TweenerCore<float, float, FloatOptions>> activeTweens = new();
    
    private void Start() {
        canvasGroup.alpha = 0f;
        mainMask.fillAmount = 0f;
        endMask.fillAmount = 0f;
        counter.target = 0;
    }

    private void Update() {
        if (!player) return;
        var count = player.opponent.comboCounter.count;
        
        if (count >= 2) {
            if (!shown) {
                StopAllCoroutines();
                StartCoroutine(Show());
            }
        }

        if (shown) {
            if (count > counter.target) {
                counter.target = count;
            } else if (count < counter.target){
                StopAllCoroutines();
                StartCoroutine(Hide());
            }
        }
        
    }

    private IEnumerator Show() {
        CancelAllTweens();
        counter.target = 1f;
        counter.ApplyImmediately();
        
        canvasGroup.alpha = 0f;
        activeTweens.Add(canvasGroup.DOFade(1, 0.4f));
        mainMask.fillAmount = 0f;
        activeTweens.Add(mainMask.DOFillAmount(1f, 0.4f));
        endMask.fillAmount = 0f;
        
        shown = true;
        yield break;
    }

    private IEnumerator Hide() {
        if (!shown) yield break;
        CancelAllTweens();
        
        shown = false;
        endMask.fillAmount = 0f;
        activeTweens.Add(endMask.DOFillAmount(1f, 0.3f));
        yield return new WaitForSeconds(1.5f);

        activeTweens.Add(mainMask.DOFillAmount(0f, 0.2f));
    }

    private void CancelAllTweens() {
        foreach (var tween in activeTweens) {
            tween.Kill();
        }
    }

}
}
