using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Input;
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
    public bool maySelect { get; set; } = true;
    public GameObject selected {
        get => _selected.gameObject;
        set {
            
            if (_selected) Deselect(_selected);
            _selected = value.GetComponent<Selectable>();
            if (_selected) Select(_selected);
        }
    }
    private Selectable _selected;
    private LocalInputModule inputModule => InputDevicePool.inst.GetInputModule(inputDevice);

    private void Start() {
        if (defaultSelected) selected = defaultSelected.gameObject;
    }

    private void Update() {
        if (_selected != null && !_selected) {
            _selected = null;
            EventSystem.current.SetSelectedGameObject(null);
        }
        
        if (!inputModule) return;

        if (_selected && maySelect && inputModule.GetAction("SysNavigate").WasPressedThisFrame()) {
            var direction = inputModule.GetAction("SysNavigate").ReadValue<Vector2>();
            Navigate(direction);
        }
    }

    public void Navigate(Vector2 direction) {
        var next = _selected.FindSelectable(direction);
        if (next) selected = next.gameObject;
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
