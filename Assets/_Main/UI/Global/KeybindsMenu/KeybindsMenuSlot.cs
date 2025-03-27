using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Scripts.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Global {
public class KeybindsMenuSlot : MonoBehaviour, ISelectHandler, IDeselectHandler {
    [Title("References")]
    public Image bg;
    public TMP_Text actionText;
    public TMP_Text keybindText;
    public Image keybindOutline;
    public Button button;
    
    public bool expanded { get; set; }
    
    public string inputModuleName { get; private set; }
    public string actionName { get; private set; }
    private RectTransform rectTransform;

    public InputAction action => inputModule.GetAction(actionName);
    public LocalInputModule inputModule => InputDevicePool.inst.GetInputModule(inputModuleName);
    
    private void Start() {
        rectTransform = transform as RectTransform;
        rectTransform.localScale = new(0, 1, 1);
    }

    private void Update() {
        rectTransform.localScale = new(Mathf.Lerp(rectTransform.localScale.x, expanded ? 1 : 0, Time.deltaTime * 10f), 1, 1);
    }

    public void Init(string actionName, string inputModuleName) {
        this.inputModuleName = inputModuleName;
        this.actionName = actionName;
        
        actionText.text = actionName;
        keybindText.text = action.GetBindingDisplayString();
    }
    
    public void OnSelect(BaseEventData eventData) {
    }
    
    public void OnDeselect(BaseEventData eventData) {
    }
}
}
