using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.UI.Generic {
public class RotaryCounter : MonoBehaviour {
    [Title("Settings")]
    public int supportedDigits;
    public RotaryCounterDigit digit;
    public int initialValue;
    public float speedThresholdScale = 1f;
    public float speedScale = 1f;
    public int freeSpinDigits = 1;
    public float borderThreshold = 1f;
    public bool useSpecialZeroes = true;
    public bool autoCreateDigits = true;
    public List<RotaryCounterDigit> digits = new();
    
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
        float firstDigit = 0;
        for (int i = 0; i < supportedDigits; i++) {
            var digit = digits[i];
            bool freeSpin = i < freeSpinDigits;
            
            digit.SetUseSpecialZeroes(Mathf.Pow(10, i + 1) > number && useSpecialZeroes);

            var digitFloat = number % Mathf.Pow(10, i + 1) / Mathf.Pow(10, i);
            if (i == 0) {
                firstDigit = digitFloat;
            }
            
            if (freeSpin) {
                digit.number = digitFloat;
                
            } else {
                // var firstDigit = GetFirstDigitAndDecimal(number);
                // Debug.Log($"{transform.parent.parent} #{i+1} threshold {(10 - borderThreshold) / 10f} diff {digitFloat % 1}");
                if ((10 - firstDigit < borderThreshold) && (digitFloat % 1 > (10 - borderThreshold) / 10f)) {
                    // rolling up
                    digit.number = (int)digitFloat + (1 - ((10 - firstDigit) / borderThreshold));
                } else {
                    digit.number = (int)digitFloat;
                }
            }
            
            // float digitValue = number % Mathf.Pow(10, order) / Mathf.Pow(10, i);
            // digit.number = (int)digitValue + (i == 0 ? (number % 1) : 0);
        }
    }

    public void ApplyImmediately() {
        current = target;
        SetNumber(current);
        foreach (var digit in digits) digit.number = current;
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
    
    private static float GetFirstDigitAndDecimal(float num) {
        return (int)(num / Math.Pow(10, (int)Math.Floor(Math.Log10(num)))) + (num % 1);
    }
}

public enum RotaryCounterDirection {
    UP,
    DOWN
}
}
