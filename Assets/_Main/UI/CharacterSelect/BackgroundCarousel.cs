using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.CharacterSelect {
public class BackgroundCarousel : MonoBehaviour {
    [Title("References")]
    public Image topImage;
    public Image bottomImage;

    [Title("Config")]
    public float transitionDuration = 1f;
    public float transitionInterval = 3f;
    public List<Sprite> backgrounds;
    
    private int index = 0;

    private void Start() {
        topImage.sprite = backgrounds[index];
        
        StartCoroutine(Loop());
    }

    private void BeginTransition(Sprite image) {
        bottomImage.sprite = topImage.sprite;
        topImage.sprite = image;

        topImage.color = topImage.color.ApplyAlpha(0);
        topImage.DOFade(1, transitionDuration);

        bottomImage.color = bottomImage.color.ApplyAlpha(1);
        bottomImage.DOFade(0, transitionDuration);
    }

    private IEnumerator Loop() {
        while (true) {
            yield return new WaitForSeconds(transitionInterval);
            ++index;
            if (index >= backgrounds.Count) {
                index = 0;
            }
            BeginTransition(backgrounds[index]);
        }
    }
}
}
