using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SuperSmashRhodes.UI.Generic {
public class CarouselDisplay : MonoBehaviour {
    [Title("Config")]
    public float fadeSpeed = 1;
    public float fadeInterval = 3;
    public bool random = false;

    private float counter;
    private int currentIndex;
    private int previousIndex;
    
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

            if (random) {
                currentIndex = Random.Range(0, transform.childCount);
            } else {
                ++currentIndex;
                if (currentIndex >= transform.childCount) {
                    currentIndex = 0;
                }   
            }
            
            Fade(transform.GetChild(previousIndex).GetComponent<CanvasGroup>(), transform.GetChild(currentIndex).GetComponent<CanvasGroup>());
            previousIndex = currentIndex;
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
