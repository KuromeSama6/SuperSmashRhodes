using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.UI.Generic {
public class CarouselDisplay : MonoBehaviour {
    [Title("Config")]
    public float fadeSpeed = 1;
    public float fadeInterval = 3;

    private float counter;
    private int currentIndex;
    
    private void Start() {
        var index = 0;
        foreach (Transform child in transform) {
            if (!child.GetComponent<CanvasGroup>()) child.gameObject.AddComponent<CanvasGroup>();

            child.GetComponent<CanvasGroup>().alpha = index == 0 ? 1 : 0;
            ++index;
        }
    }

    private void Update() {
        counter += Time.deltaTime;
        
        if (counter >= fadeInterval) {
            counter = 0;
            ++currentIndex;
            if (currentIndex >= transform.childCount) {
                currentIndex = 0;
            }
            
            Fade(transform.GetChild(currentIndex == 0 ? transform.childCount - 1 : currentIndex - 1).GetComponent<CanvasGroup>(), transform.GetChild(currentIndex).GetComponent<CanvasGroup>());
        }
    }

    private void Fade(CanvasGroup from, CanvasGroup to) {
        foreach (Transform child in transform) {
            child.GetComponent<CanvasGroup>().alpha = 0;
        }
        
        from.alpha = 1;
        to.alpha = 0;

        LeanTween.alphaCanvas(from, 0, fadeSpeed);
        LeanTween.alphaCanvas(to, 1, fadeSpeed);
    }
}
}
