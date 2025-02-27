using System;
using SuperSmashRhodes.Battle;
using UnityEngine;

namespace SuperSmashRhodes.UI {
/**
 * A UI element that is specific to a particular character component.
 * This element will automatically hide/show itself based on the presence of the component on the character.
 */
public class ComponentSpecificUIElement<T> : PerSideUIElement<ComponentSpecificUIElement<T>> where T: CharacterComponent {
    public T playerComponent {
        get {
            if (_playerComponent) return _playerComponent;
            if (!player) return null;
            _playerComponent = player.GetComponent<T>();
            return _playerComponent;
        }
    }
    private T _playerComponent;

    protected CanvasGroup canvasGroup { get; private set; }
    
    protected virtual void Start() {
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }
    
    protected virtual void Update() {
        bool show = playerComponent;
        canvasGroup.alpha = show ? 1 : 0;
        canvasGroup.interactable = show;
        canvasGroup.blocksRaycasts = show;
    }

    public void ResetState() {
        _playerComponent = null;
    }
}
}
