using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SuperSmashRhodes.UI.Generic {
public class RotaryCounter : MonoBehaviour {
    [Title("Settings")]
    public int supportedDigits;
    public RotaryCounterDigit digit;
    public int initialValue;
    public float speedThresholdScale = 1f;
    public float speedScale = 1f;
    public bool useSpecialZeroes = true;
    public bool autoCreateDigits = true;
    public List<RotaryCounterDigit> digits = new();
    
    public RotaryCounterDirection direction { get; set; } = RotaryCounterDirection.DOWN;
    public float target { get; set; }
    public float current { get; private set; }

    private void Start() {
        if (autoCreateDigits) {
            digits.Clear();
            for (int i = 0; i < supportedDigits; i++) {
                var newDigit = Instantiate(digit.gameObject, transform).GetComponent<RotaryCounterDigit>();
            
                digits.Add(newDigit);
                newDigit.transform.SetAsFirstSibling();
            }
            digit.gameObject.SetActive(false);
        }
        
        target = initialValue;
        ApplyImmediately(); 
    }

    private void Update() {
        // if (UnityEngine.Input.GetKeyDown(KeyCode.Minus)) target -= 30;
        
        if (!Mathf.Approximately(target, current)) direction = target > current ? RotaryCounterDirection.UP : RotaryCounterDirection.DOWN;
        var step = Time.deltaTime * GetRotarySpeed(1);
        
        if (Math.Abs(current - target) > step) {
            if (current > target) {
                current -= step;

            } else {
                current += step;
            }
            
        } else current = target;
        
        SetNumber(current);
    }

    public void SetNumber(float number) {
        for (int i = 0; i < supportedDigits; i++) {
            var digit = digits[i];
            digit.speed = GetRotarySpeed(i + 1);
            digit.SetUseSpecialVeroes(Mathf.Pow(10, i + 1) > number && useSpecialZeroes);
            
            float digitValue = number % Mathf.Pow(10, i + 1) / Mathf.Pow(10, i);
            // Debug.Log($"{i}: {digitValue}");
            
            digit.number = (int)digitValue + (i == 0 ? (number % 1) : 0);
        }
    }

    public void ApplyImmediately() {
        current = target;
        SetNumber(current);
        foreach (var digit in digits) digit.ApplyImmediately();
    }

    private float GetRotarySpeed(int order) {
        var speed = 5f;
        var multiplier = Mathf.Pow(order, 10) * speedScale;
        
        var delta = Mathf.Abs(target - current);
        if (delta >= 10 * speedThresholdScale * multiplier) speed = 10f;
        if (delta >= 100 * speedThresholdScale * multiplier) speed = 32f;
        if (delta >= 1000 * speedThresholdScale * multiplier) speed = 64f;
        return speed * speedScale;
    }
}

public enum RotaryCounterDirection {
    UP,
    DOWN
}
}
