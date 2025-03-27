using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Framework.UI;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Scripts.Audio;
using SuperSmashRhodes.Settings;
using SuperSmashRhodes.UI.Toast;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;
using Object = UnityEngine.Object;

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
    private InputAction rebindingAction;
    private bool listeningForRebind;
    private Object listenerPlaceholder;
    private int lastSelected;
    private IDisposable inputListener;
    private bool bindingInputLock;
    
    private void Start() {
        ReloadSlots();
        canvasGroup.alpha = 0;
        visible = false;
        navigatableMenu = GetComponent<NavigatableMenu>();

        listenerPlaceholder = ScriptableObject.CreateInstance<ScriptableObject>();

        inputListener = InputSystem.onAnyButtonPress.Call(OnAnyButtonPress);
    }

    private void Update() {
        if (visible && !inputModule) {
            Close();
        }
        
        if (!inputModule) return;
        
        //TODO Remove Debug
        if (visible) {
            if (InputDevicePool.inst.IsTopOfStack(this) && !bindingInputLock) {
                if (inputModule.GetAction("SysBack").WasPressedThisFrame()) Close();
                
                if (inputModule.GetAction("SysConfirm").WasPressedThisFrame()) {
                    var slot = navigatableMenu.selected.GetComponentInChildren<KeybindsMenuSlot>();
                    ShowBindPrompt(slot);
                }
                
            }

            if (navigatableMenu.selected) {
                lastSelected = navigatableMenu.selected.transform.GetSiblingIndex();
            }
            navigatableMenu.maySelect = InputDevicePool.inst.IsTopOfStack(this);
            bindingInputLock = listeningForRebind;
            
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
        
        this.CallNextFrameCoroutine(() => navigatableMenu.selected = container.GetChild(lastSelected).GetComponent<Selectable>());
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

    private void OnAnyButtonPress(InputControl control) {
        if (!listeningForRebind) return;
        if (control.device != inputModule.device) return;
        
        // check for excape
        if (MatchesInputAction(inputModule.GetAction("SysMenu"), control, inputModule.input.currentControlScheme)) {
            // cancel
            GlobalPrompt.inst.Cancel();
            return;
        }
        
        // find occupied button
        bool keySwapped = false;
        foreach (var input in InputDevicePool.inst.inputs.Values) {
            var actions = inputModule.input.actions;
            var occupied = actions.Where(c => MatchesInputAction(c, control)).FirstOrDefault();
            if (occupied != null) {
                var binding = occupied.bindings.First(c => c.groups.Contains(input.input.currentControlScheme));
                var conflict = InputControlPath.Matches(binding.effectivePath, control);

                if (conflict) {
                    // if (!mappableKeys.Any(c => c.action.name == occupied.name)) {
                    //     GlobalPrompt.inst.Cancel(false);
                    //     GlobalPrompt.inst.ShowPrompt("不能绑定该按键。", GlobalPromptFlags.CONFIRM);
                    //     return;
                    // } else 
                    if (input == inputModule) {
                        if (mappableKeys.Any(c => c.action.name == occupied.name)) {
                            var occupiedIndex = occupied.bindings.IndexOf(c => c.groups.Contains(inputModule.input.currentControlScheme));
                            if (occupiedIndex == -1) return;
                            occupied.ApplyBindingOverride(occupiedIndex, rebindingAction.bindings[occupiedIndex].effectivePath);   
                            keySwapped = true;
                        }
                        
                    } else {
                        // GlobalPrompt.inst.Cancel(false);
                        // GlobalPrompt.inst.ShowPrompt("无法绑定该按键。\n该按键已被绑定至其他设备。", GlobalPromptFlags.CONFIRM);
                        ToastManager.inst.ShowToast(ToastType.WARNING, "无法绑定该按键", "该按键已被绑定至其他设备");
                        return;   
                    }
                }
            }   
        }
        
        // bind
        var index = rebindingAction.bindings.IndexOf(c => c.groups.Contains(inputModule.input.currentControlScheme));
        if (index == -1) return;
        
        rebindingAction.ApplyBindingOverride(index, control.path);
        
        GlobalPrompt.inst.Confirm();
        listeningForRebind = false;
        ReloadSlots();
        InputDevicePool.inst.PopOverlay(listenerPlaceholder);
        this.CallNextFrameCoroutine(() => navigatableMenu.selected = container.GetChild(lastSelected).GetComponent<Selectable>());

        if (keySwapped) {
            ToastManager.inst.ShowToast(ToastType.ALERT, "成功", "有至少一个按键被替换");
        } else {
            ToastManager.inst.ShowToast(ToastType.SUCCESS, "成功", "成功修改绑定");
        }
        
        SettingsManager.inst.data.playerInputSettings = inputModule.input.actions.SaveBindingOverridesAsJson();
        SettingsManager.inst.Save();
    }
    
    private void ShowBindPrompt(KeybindsMenuSlot slot) {
        // begin rebind
        var action = slot.action;
        var binding = action.bindings.IndexOf(c => c.groups.Contains(inputModule.input.currentControlScheme));
        if (binding == -1) return;
        
        AudioManager.inst.PlayAudioClip("cmn/sfx/ui/button/normal", gameObject);
        GlobalPrompt.inst.ShowPrompt($"正在修改: {slot.action.name}\n\n按下希望绑定的按键。 按下菜单键以取消。", GlobalPromptFlags.NONE,
                                     () => { },
                                     () => {
                                            listeningForRebind = false;
                                            InputDevicePool.inst.PopOverlay(listenerPlaceholder);
                                     }
        );

        rebindingAction = action;
        listeningForRebind = true;
    }
    
    private void ReloadSlots() {
        container.gameObject.ClearChildren();
        slots.Clear();
        
        if (!inputModule) return;

        foreach (var action in mappableKeys) {
            var go = Instantiate(slotPrefab, container);
            var slot = go.GetComponent<KeybindsMenuSlot>();
            slot.Init(action.name, inputModule.schemeName);
            
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

    private void OnDestroy() {
        inputListener.Dispose();
    }

    public static bool MatchesInputAction(InputAction action, InputControl control, string controlScheme = null) {
        var bindings = action.bindings
            .Where(c => controlScheme == null || c.groups.Contains(controlScheme))
            .Where(c => !c.isComposite)
            .ToList();

        return bindings.Any(c => InputControlPath.Matches(c.effectivePath, control));
    }
}
}
