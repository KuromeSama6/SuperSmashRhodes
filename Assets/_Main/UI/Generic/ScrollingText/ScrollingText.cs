using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Generic {
public class ScrollingText : MonoBehaviour {
    [Title("References")]
    public TMP_Text mainText;
    public RectTransform container;
    public HorizontalLayoutGroup layoutGroup;
    public float scrollSpeed = 1f;
    public string displayText;

    private readonly List<TMP_Text> texts = new();
    private float textWidth => mainText.rectTransform.rect.width;
    private bool dirty;

    public string text {
        get => mainText.text;
        set {
            if (value == mainText.text) return;
            mainText.text = value;
            EnsureTexts();
            LayoutRebuilder.ForceRebuildLayoutImmediate(container);
        }
    }
    
    private void Start() {
        texts.Add(mainText);
        dirty = true;
        displayText = "";
    }

    private void Update() {
        var minimumWidth = container.rect.width * 2f;
        if (minimumWidth == 0 || textWidth == 0) return;
        if (dirty) {
            EnsureTexts();
            LayoutRebuilder.ForceRebuildLayoutImmediate(container);
            dirty = false;
        }
        
        if (displayText.Length > 0) {
            text = displayText;
            displayText = "";
        }
        
        container.anchoredPosition -= new Vector2(scrollSpeed * Time.deltaTime, 0);
        var textCount = Mathf.CeilToInt(container.rect.width / textWidth);
        var threshold =  textCount * (textWidth + layoutGroup.spacing);
        // Debug.Log($"th {threshold} w {textWidth} spa {layoutGroup.spacing}");

        if (container.anchoredPosition.x < -threshold) {
            container.anchoredPosition = Vector2.zero;
        } else if (container.anchoredPosition.x > 0) {
            container.anchoredPosition = new Vector2(-threshold, 0);
        }
    }

    public void EnsureTexts() {
        var minimumWidth = container.rect.width * 2f;
        if (minimumWidth == 0 || textWidth == 0) return;
        var required = Mathf.CeilToInt(minimumWidth / textWidth);
        // Debug.Log($"minw {minimumWidth} req {required} cont {container.rect.width}");

        foreach (var text in texts.ToList()) {
            if (text == mainText) continue;
            Destroy(text.gameObject);
            texts.Remove(text);
        }

        for (int i = 0; i < required - 1; i++) {
            var go = Instantiate(mainText.gameObject, container);
            texts.Add(go.GetComponent<TMP_Text>());
        }

        container.anchoredPosition = Vector2.zero;
    }
}
}
