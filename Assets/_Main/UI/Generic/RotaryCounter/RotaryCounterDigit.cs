using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace SuperSmashRhodes.UI.Generic {
public class RotaryCounterDigit : MonoBehaviour {
    [Title("References")]
    public RectTransform digitText;
    public RectTransform container;
    public List<GameObject> specialZeroes = new();
    
    public float number { get; set; }
    
    private float height;
    private RotaryCounter counter;
    
    private void Start() {
        counter = GetComponentInParent<RotaryCounter>();
        
    }

    private void Update() {
        if (height == 0) {
            var newHeight = digitText.rect.height;
            if (newHeight == 0) return;
            
            height = newHeight * 11f;
            container.sizeDelta = new Vector2(container.sizeDelta.x, newHeight);
            container.anchoredPosition = new(container.anchoredPosition.x, 0);
        }
        
        // var direction = counter.direction;
        var y = height / 11f * (10 - number);
        var pos = container.anchoredPosition;
        pos.y = y;
        container.anchoredPosition = pos;
    }

    public void SetUseSpecialZeroes(bool use) {
        foreach (var zero in specialZeroes) zero.SetActive(use);
    }
}

}
