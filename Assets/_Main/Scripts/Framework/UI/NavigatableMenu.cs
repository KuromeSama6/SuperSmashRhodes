using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Scripts.Audio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SuperSmashRhodes.Framework.UI {
public class NavigatableMenu : MonoBehaviour {
    [Title("References")]
    public Selectable defaultSelected;

    public string inputDevice { get; set; }
    public bool maySelect { get; set; }
    public Selectable selected {
        get {
            return _selected;
        }
        set {
            if (_selected) Deselect(_selected);
            _selected = value;
            if (_selected) Select(_selected);
        }
    }
    private Selectable _selected;
    private LocalInputModule inputModule => InputDevicePool.inst.GetInputModule(inputDevice);

    private void Start() {
        if (defaultSelected) selected = defaultSelected;
    }

    private void Update() {
        if ((object)_selected != null && !_selected) {
            selected = null;
        }
        
        if (!inputModule) return;

        if (_selected && maySelect && inputModule.GetAction("SysNavigate").WasPressedThisFrame()) {
            var direction = inputModule.GetAction("SysNavigate").ReadValue<Vector2>();
            Navigate(direction);
            AudioManager.inst.PlayAudioClip("cmn/ui/navigate/normal", gameObject);
        }
    }

    public void Navigate(Vector2 direction) {
        var next = _selected.FindSelectable(direction);
        if (next) selected = next;
    }
    
    public static void Select(Selectable selectable) {
        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(selectable.gameObject, pointer, ExecuteEvents.selectHandler);
    }

    public static void Deselect(Selectable selectable) {
        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(selectable.gameObject, pointer, ExecuteEvents.deselectHandler);
    }
}
}
