using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SuperSmashRhodes.Input {
public class LocalInputManager : SingletonBehaviour<LocalInputManager> {
    [Title("References")]
    public InputActionAsset inputSettings;
    public GameObject inputModulePrefab;

    private GameObject container;
    private Dictionary<String, LocalInputModule> localInputModules { get; } = new();
    
    private void Start() {
        {
            // create local input modules
            container = new GameObject("InputModules");
            container.transform.parent = transform;
            var devices = InputSystem.devices;
            
            foreach (var scheme in inputSettings.controlSchemes) {
                using (var candidates = scheme.PickDevicesFrom(devices: devices)) {
                    if (!candidates.isSuccessfulMatch) {
                        Debug.LogError($"No device found for control scheme {scheme.name}");
                        continue;
                    }
                
                    var module = PlayerInput.Instantiate(inputModulePrefab, controlScheme: scheme.name, pairWithDevice: candidates.devices[0]);
                    module.transform.parent = container.transform;
                    module.name = $"InputModule_{scheme.name}";
                
                    localInputModules[scheme.name] = module.GetComponent<LocalInputModule>();   
                }
            }
        }
    }

    public IInputProvider GetInputProvider(PlayerCharacter player) {
        //TODO: Change debug
        if (player.playerIndex == 0) {
            return localInputModules["keyboard1"];
        }
        return new NOPInputProvider();
    }
}
}
