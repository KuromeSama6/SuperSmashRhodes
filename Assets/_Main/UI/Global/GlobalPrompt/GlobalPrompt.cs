using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Framework.UI;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Scripts.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Global {
public class GlobalPrompt : SingletonBehaviour<GlobalPrompt>, ISelectHandler {
    [Title("References")]
    public CanvasGroup canvasGroup;
    public RectTransform container;
    public TMP_Text text;
    public Button cancelBtn, confirmBtn;
    
    public GlobalPromptData? currentPrompt { get; private set; }
    
    private void Start() {
        canvasGroup.alpha = 0;
        
        cancelBtn.onClick.AddListener(() => Cancel());
        confirmBtn.onClick.AddListener(() => Confirm());
    }

    private void Update() {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, currentPrompt.HasValue ? 1 : 0, Time.deltaTime * 20f);
        // container.localScale = new(1, Mathf.Lerp(container.localScale.y, currentPrompt.HasValue ? 1 : 0, Time.deltaTime * 10f), 1);

        if (!currentPrompt.HasValue) return;
        
        if (InputDevicePool.inst.IsTopOfStack(this)) {
            var input = InputDevicePool.inst.GetInputModule(currentPrompt.Value.inputDevice);
            if (input.GetAction("SysConfirm").WasPressedThisFrame() && currentPrompt.Value.flags.HasFlag(GlobalPromptFlags.CONFIRM)) {
                Confirm();
            }
            
            if (input.GetAction("SysBack").WasPressedThisFrame() && currentPrompt.Value.flags.HasFlag(GlobalPromptFlags.CANCEL)) {
                Cancel();
            }
        }
        
        
    }

    public void ShowPrompt(string text, GlobalPromptFlags flags = GlobalPromptFlags.REGULAR, Action onConfirm = null, Action onCancel = null, string inputDevice = null) {
        if (currentPrompt.HasValue) ClosePrompt();
        if (!flags.HasFlag(GlobalPromptFlags.NO_SOUND)) AudioManager.inst.PlayAudioClip("cmn/sfx/ui/prompt/normal", gameObject);
        
        currentPrompt = new GlobalPromptData {
            text = text,
            flags = flags,
            onConfirm = onConfirm,
            onCancel = onCancel,
            inputDevice = inputDevice
        };
        
        this.text.text = text;
        
        cancelBtn.gameObject.SetActive(flags.HasFlag(GlobalPromptFlags.CANCEL));
        confirmBtn.gameObject.SetActive(flags.HasFlag(GlobalPromptFlags.CONFIRM));
        
        InputDevicePool.inst.PushOverlay(this);
    }
    
    public void Cancel(bool playSound = true) {
        if (!currentPrompt.HasValue) return;
        if (playSound) AudioManager.inst.PlayAudioClip("cmn/sfx/ui/button/back", gameObject);
        if (currentPrompt.Value.onCancel != null) currentPrompt.Value.onCancel.Invoke();
        ClosePrompt();
    }
    
    public void Confirm(bool playSound = true) {
        if (!currentPrompt.HasValue) return;
        if (playSound) AudioManager.inst.PlayAudioClip("cmn/sfx/ui/button/heavy", gameObject);
        if (currentPrompt.Value.onConfirm != null) currentPrompt.Value.onConfirm.Invoke();
        ClosePrompt();
    }
    
    private void ClosePrompt() {
        if (!currentPrompt.HasValue) return;
        currentPrompt = null;
        InputDevicePool.inst.PopOverlay(this);
    }
    
    public void OnSelect(BaseEventData eventData) {
        AudioManager.inst.PlayAudioClip("cmn/ui/navigate/normal", gameObject);
    }
}

public struct GlobalPromptData {
    public string text;
    public GlobalPromptFlags flags;
    public Action onConfirm, onCancel;
    public string inputDevice;
}

[Flags]
public enum GlobalPromptFlags {
    NONE = 0,
    CANCEL = 1 << 0,
    CONFIRM = 1 << 1,
    NO_SOUND = 1 << 2,
    
    REGULAR = CANCEL | CONFIRM
}
}
