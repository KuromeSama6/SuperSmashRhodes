using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Framework.UI;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Scripts.Audio;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SuperSmashRhodes.UI.Global {
public class KeybindsMenuUI : SingletonBehaviour<KeybindsMenuUI> {
    [Title("References")]
    public CanvasGroup canvasGroup;
    public RectTransform container;
    public GameObject slotPrefab;

    [Title("Config")]
    public List<InputActionReference> mappableKeys = new();
    
    public bool visible { get; private set; }
    
    public string inputDevice { get; private set; }
    private readonly List<KeybindsMenuSlot> slots = new();
    private NavigatableMenu navigatableMenu;
    private LocalInputModule inputModule => InputDevicePool.inst.GetInputModule(inputDevice);

    private void Start() {
        ReloadSlots();
        canvasGroup.alpha = 0;
        visible = false;
        navigatableMenu = GetComponent<NavigatableMenu>();
    }

    private void Update() {
        if (visible && !inputModule) {
            Close();
        }
        
        if (!inputModule) return;
        
        //TODO Remove Debug
        if (visible) {
            if (InputDevicePool.inst.IsTopOfStack(this)) {
                if (inputModule.GetAction("SysMenu").WasPressedThisFrame() || inputModule.GetAction("SysBack").WasPressedThisFrame()) Close();
                
                if (inputModule.GetAction("SysConfirm").WasPressedThisFrame()) {
                    var slot = navigatableMenu.selected.GetComponentInChildren<KeybindsMenuSlot>();
                    ShowBindPrompt(slot);
                }
            }
            
            navigatableMenu.maySelect = InputDevicePool.inst.IsTopOfStack(this);
            
        } else {
            foreach (var input in InputDevicePool.inst.inputs.Values) {
                if (input.GetAction("SysMenu").WasPressedThisFrame()) {
                    Open(input.input.currentControlScheme);
                    break;
                }
            }
        }
        
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, visible ? 1 : 0, Time.deltaTime * 20f);
    }

    public void Open(string inputDevice) {
        this.inputDevice = inputDevice;
        ReloadSlots();
        visible = true;
        
        this.CallNextFrameCoroutine(() => navigatableMenu.selected = container.GetChild(0).gameObject);
        navigatableMenu.inputDevice = inputDevice;
        
        AudioManager.inst.PlayAudioClip("cmn/sfx/ui/button/normal", gameObject);
        InputDevicePool.inst.PushOverlay(this);
    }

    public void Close() {
        visible = false;
        inputDevice = null;
        AudioManager.inst.PlayAudioClip("cmn/sfx/ui/button/back", gameObject);
        InputDevicePool.inst.PopOverlay(this);
    }

    private void ShowBindPrompt(KeybindsMenuSlot slot) {
        GlobalPrompt.inst.ShowPrompt($"正在修改: {slot.action.name}\n\n按下希望绑定的按键", GlobalPromptFlags.CANCEL);
    }
    
    private void ReloadSlots() {
        container.gameObject.ClearChildren();
        slots.Clear();
        
        if (!inputModule) return;

        foreach (var action in mappableKeys) {
            var go = Instantiate(slotPrefab, container);
            var slot = go.GetComponent<KeybindsMenuSlot>();
            slot.Init(action.name, inputModule);
            
            slots.Add(slot);
        }
        
        StartCoroutine(SequenceExpandSlots(0.025f));
    }

    private IEnumerator SequenceExpandSlots(float interval) {
        foreach (var slot in slots) {
            slot.expanded = false;
        }

        foreach (var slot in slots) {
            slot.expanded = true;
            yield return new WaitForSeconds(interval);
        }
    }
}
}
