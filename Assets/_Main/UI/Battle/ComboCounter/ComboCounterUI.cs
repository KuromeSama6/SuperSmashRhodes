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
    private int lastCount = 0;
    private float sizeTarget = 1f;
    private float sizeCurrent = 1f;
    
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
                if (lastCount < count) {
                    OnIncrement();
                    lastCount = count;
                }
                
            } else if (count < counter.target){
                StopAllCoroutines();
                StartCoroutine(Hide());
            }
        }

        sizeCurrent = Mathf.Lerp(sizeCurrent, sizeTarget, Time.deltaTime * 15f);
        transform.localScale = Vector3.one * sizeCurrent;
    }

    private void OnIncrement() {
        sizeTarget = Mathf.Min(3f, sizeTarget + .1f);
        sizeCurrent += .5f;
    }
    
    private IEnumerator Show() {
        CancelAllTweens();
        counter.target = 1f;
        lastCount = 1;
        sizeTarget = sizeCurrent = 1f;
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

        transform.localScale = Vector3.one;
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
