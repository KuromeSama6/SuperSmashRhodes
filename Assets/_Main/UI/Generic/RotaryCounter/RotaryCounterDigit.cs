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
    public float speed { get; set; } = 2f;
    
    private float height;
    private float current;
    private RotaryCounter counter;
    
    private void Start() {
        counter = GetComponentInParent<RotaryCounter>();
        
        current = number;
    }

    private void Update() {
        if (height == 0) {
            var newHeight = digitText.rect.height;
            if (newHeight == 0) return;
            
            height = newHeight * 11f;
            container.sizeDelta = new Vector2(container.sizeDelta.x, newHeight);
            container.anchoredPosition = new(container.anchoredPosition.x, 0);
        }
        
        var direction = counter.direction;
        var target = number;

        var step = Time.deltaTime * speed;
        if (Math.Abs(current - target) > step) {
            if (direction == RotaryCounterDirection.DOWN) {
                current -= step;
                if (current <= 0) current = 10;

            } else {
                current += step;
                if (current >= 10) current = 0;
            }
            
        } else current = target;

        var y = height / 11f * current;
        var pos = container.anchoredPosition;
        pos.y = y;
        container.anchoredPosition = pos;
    }

    public void SetUseSpecialVeroes(bool use) {
        foreach (var zero in specialZeroes) zero.SetActive(use);
    }
    
    public void ApplyImmediately() {
        current = number;
    }
}

}
