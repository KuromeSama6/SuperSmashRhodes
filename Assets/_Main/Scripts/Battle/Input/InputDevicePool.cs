using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace SuperSmashRhodes.Input {
public class InputDevicePool : SingletonBehaviour<InputDevicePool> {
    [Title("References")]
    public InputActionAsset inputSettings;
    public GameObject inputModulePrefab;

    private PlayerInputManager inputManager;
    private GameObject container;
    public readonly Dictionary<string, LocalInputModule> inputs = new();

    private readonly List<InputDevice> recentDeviceStack = new();
    private readonly List<Object> overlayStack = new();
    
    public string currentActionMap {
        get => _currentActionMap;
        set {
            if (_currentActionMap == value) {
                return;
            }
            
            _currentActionMap = value;
            ReloadLocalInput();
        }
    }
    private string _currentActionMap;

    public LocalInputModule defaultInput {
        get {
            if (recentDeviceStack.Count == 0) return inputs["keyboard1"];
            var ret = recentDeviceStack[0];
            return inputs.Values.FirstOrDefault(i => i.device == ret);
        }
    }
    
    public bool hasGameBaseInput => overlayStack.Count == 0; 
    
    protected override void Awake() {
        base.Awake();
        inputManager = GetComponent<PlayerInputManager>();
        InputSystem.onDeviceChange += OnDeviceChanged;
        container = new GameObject("InputModules");
        container.transform.parent = transform;
        
        ReloadLocalInput(); 
        
    }

    private void Start() {
    }

    private void Update() {
        overlayStack.RemoveAll(c => !c);
    }

    public void ReloadLocalInput() {
        container.ClearChildren();
        inputs.Clear();
        
        // create local input modules
        var devices = InputSystem.devices;
            
        foreach (var scheme in inputSettings.controlSchemes) {
            using (var candidates = scheme.PickDevicesFrom(devices)) {
                if (!candidates.isSuccessfulMatch) {
                    continue;
                }
                
                var module = PlayerInput.Instantiate(inputModulePrefab, controlScheme: scheme.name, pairWithDevice: candidates.devices[0]);
                module.transform.parent = container.transform;
                module.name = $"InputModule_{scheme.name}";
                module.SwitchCurrentActionMap(_currentActionMap ?? "Player");
                
                var input = module.GetComponent<LocalInputModule>();
                input.device = candidates.devices[0];
                inputs[scheme.name] = input;
            }
        }
    }

    public void PushOverlay(Object overlay) {
        if (overlayStack.Contains(overlay)) return;
        overlayStack.Insert(0, overlay);
    }
    
    public void PopOverlay(Object overlay) {
        overlayStack.Remove(overlay);
    }

    public bool IsTopOfStack(Object obj) {
        return overlayStack.Count > 0 && overlayStack[0] == obj;
    }
    
    private void OnDeviceChanged(InputDevice device, InputDeviceChange change) {
        ReloadLocalInput();
        
        // Debug.Log($"device {device} changed: {change}");
        if (change == InputDeviceChange.Added) {
            recentDeviceStack.Insert(0, device);
            
        } else if (change == InputDeviceChange.Removed || change == InputDeviceChange.Disconnected) {
            recentDeviceStack.Remove(device);
        }
    }
    
    public IInputProvider GetInputProvider(PlayerCharacter player) {
        //TODO: Change debug
        if (player.playerIndex == 0) {
            return inputs["keyboard1"];
        } else {
            return inputs["keyboard2"];
        }
        return new NOPInputProvider();
    }

    public LocalInputModule GetInputModule(string controlScheme) {
        if (controlScheme == null) return defaultInput;
        return inputs.GetValueOrDefault(controlScheme, defaultInput);
    }

    private void OnDestroy() {
        InputSystem.onDeviceChange -= OnDeviceChanged;
    }
}
}
