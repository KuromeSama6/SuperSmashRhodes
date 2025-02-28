using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SuperSmashRhodes.Input {
public class InputDevicePool : SingletonBehaviour<InputDevicePool> {
    [Title("References")]
    public InputActionAsset inputSettings;
    public GameObject inputModulePrefab;

    private PlayerInputManager inputManager;
    private GameObject container;
    public Dictionary<String, LocalInputModule> inputs { get; } = new();

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

    public void ReloadLocalInput() {
        container.ClearChildren();
        inputs.Clear();
        
        // create local input modules
        var devices = InputSystem.devices;
            
        foreach (var scheme in inputSettings.controlSchemes) {
            using (var candidates = scheme.PickDevicesFrom(devices: devices)) {
                if (!candidates.isSuccessfulMatch) {
                    continue;
                }
                
                var module = PlayerInput.Instantiate(inputModulePrefab, controlScheme: scheme.name, pairWithDevice: candidates.devices[0]);
                module.transform.parent = container.transform;
                module.name = $"InputModule_{scheme.name}";
                module.SwitchCurrentActionMap(_currentActionMap ?? "Player");
                
                inputs[scheme.name] = module.GetComponent<LocalInputModule>();   
            }
        }
    }
    
    private void OnDeviceChanged(InputDevice device, InputDeviceChange change) {
        ReloadLocalInput();
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

    private void OnDestroy() {
        InputSystem.onDeviceChange -= OnDeviceChanged;
    }
}
}
