using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Toast {
public class Toast : MonoBehaviour {
    [Title("References")]
    public Image icon;
    public TMP_Text title, content;
    public Image progressBar;
    public UnityEngine.UI.Outline outline;
    public CanvasGroup canvas;

    public UDictionary<ToastType, Sprite> icons;
    public UDictionary<ToastType, Color> colors;
    
    public Color color {
        get => _color;
        set {
            _color = value;
            title.color = progressBar.color = outline.effectColor = icon.color = value;
            content.color = Color.Lerp(Color.black, Color.white, 0.8f);
        }
    }

    private Color _color;
    private float lifetime;
    private float initialLifetime;
    private bool destroyed;

    private void Start() {
        transform.localScale = new(0, 1, 1);
        canvas.alpha = 0f;
        transform.SetAsFirstSibling();
        
        LeanTween.scaleX(gameObject, 1, .25f).setEase(LeanTweenType.easeInOutQuint);
        LeanTween.alphaCanvas(canvas, 1f, .25f).setEase(LeanTweenType.easeInOutQuint);
    }

    public void Init(ToastType type, string title, string content, float lifetime) {
        color = colors[type];
        icon.sprite = icons[type];
        this.title.text = title;
        this.content.text = content;
        this.lifetime = initialLifetime = lifetime;
    }
    
    private void Update() {
        if (destroyed) return;
        
        lifetime -= Time.deltaTime;
        progressBar.fillAmount = lifetime / initialLifetime;
        
        if (lifetime < 0) {
            destroyed = true;
            LeanTween.scaleX(gameObject, 0, .25f).setEase(LeanTweenType.easeInOutQuint);
            LeanTween.alphaCanvas(canvas, 0f, .25f).setEase(LeanTweenType.easeInOutQuint).setDestroyOnComplete(true);
        }
    }

}

public enum ToastType {
    REGULAR,
    ALERT,
    WARNING,
    ERROR,
    SUCCESS
}
}
