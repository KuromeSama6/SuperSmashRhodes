using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SuperSmashRhodes.UI.Generic {
[ExecuteInEditMode]
public class CarouselImage : MonoBehaviour {
    [Title("Config")]
    public float fadeSpeed = 1;
    public float fadeInterval = 3;
    public bool random = false;
    public List<Sprite> sprites = new();

    [Title("Image Settings")]
    public Color color = Color.white;
    
    [Title("References")]
    public Image top;
    public Image bottom;
    
    private float counter;
    private int currentIndex;
    private int previousIndex;
    
    private void Start() {
        top.color = top.color.ApplyAlpha(color.a);
        bottom.color = bottom.color.ApplyAlpha(0);
        if (sprites.Count > 0) top.sprite = sprites[0];
    }

    private void Update() {
        top.color = top.color.ApplyAlpha(top.color.a);
        bottom.color = bottom.color.ApplyAlpha(bottom.color.a);
        
        // set image preview in editor
        if (!Application.isPlaying) {
            if (sprites.Count > 0) {
                top.sprite = sprites[0];
                top.color = top.color.ApplyAlpha(color.a);
                bottom.color = bottom.color.ApplyAlpha(0);
                
            } else {
                top.sprite = bottom.sprite = null;
            }
            return;
        }
        
        counter += Time.deltaTime;
        
        if (counter >= fadeInterval) {
            counter = 0;

            if (random) {
                currentIndex = Random.Range(0, sprites.Count);
                
            } else {
                ++currentIndex;
                if (currentIndex >= sprites.Count) {
                    currentIndex = 0;
                }   
            }
            
            // Fade(transform.GetChild(currentIndex == 0 ? transform.childCount - 1 : currentIndex - 1).GetComponent<CanvasGroup>(), transform.GetChild(currentIndex).GetComponent<CanvasGroup>());
            Fade(sprites[previousIndex], sprites[currentIndex]);
            previousIndex = currentIndex;
        }
    }

    private void Fade(Sprite from, Sprite to) {
        top.sprite = to;
        bottom.sprite = from;
        top.color = top.color.ApplyAlpha(0);
        bottom.color = bottom.color.ApplyAlpha(color.a);
        
        LeanTween.alpha(top.rectTransform, color.a, fadeSpeed);
        LeanTween.alpha(bottom.rectTransform, 0, fadeSpeed);
    }
}
}
