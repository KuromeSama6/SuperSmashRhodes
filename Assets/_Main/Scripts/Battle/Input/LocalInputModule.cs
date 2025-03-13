using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SuperSmashRhodes.Input {
[RequireComponent(typeof(PlayerInput))]
public class LocalInputModule : MonoBehaviour, IInputProvider, IEngineUpdateListener {
    [Title("Config")]
    public UDictionary<InputActionReference, InputType> inputMap = new();
        
    public InputBuffer inputBuffer => localBuffer;

    public PlayerInput input { get; private set; }
    public InputDevice device { get; set; }
    
    private List<InputFrame> thisFrameInputs = new();
    private InputBuffer localBuffer;
    
    private void Start() {
        if (FightEngine.inst) FightEngine.inst.RefreshComponentReferences();
        
        input = GetComponent<PlayerInput>();
        localBuffer = new(120);
        
        // load inputs
        {
            var file = new FileInfo(Path.Join(Application.persistentDataPath, "/local/settings/inputs.json"));
            if (file.Exists) {
                
            } else {
                file.Directory.Create();
                var json = input.actions.ToJson();
                File.WriteAllText(file.FullName, json);
            }
        }
    }

    public void ManualUpdate() {
        
    } 
    
    public InputAction this[string action] => input.actions[action];

    public void EngineUpdate() {
        
    }
    
    public void SetBuffer(InputBuffer newBuffer) {
        localBuffer = newBuffer;
    }

    public InputAction GetAction(string name) {
        return input.actions.FindAction(name);
    }
    
    private void Update() {
        if (!input) return;

        if (InputDevicePool.inst.hasGameBaseInput) {
            foreach (var (actionReference, type) in inputMap) {
                var action = input.actions.FindAction(actionReference.name);
                if (action == null) continue;

                if (action.WasPressedThisFrame()) {
                    var frame = new InputFrame(type, InputFrameType.PRESSED);
                    if (!thisFrameInputs.Contains(frame)) thisFrameInputs.Add(frame);
                }
            
                if (action.WasReleasedThisFrame()) {
                    var frame = new InputFrame(type, InputFrameType.RELEASED);
                    if (!thisFrameInputs.Contains(frame)) thisFrameInputs.Add(frame);
                }
            }

            if (input.actions.FindAction("BtlBurst").WasPressedThisFrame()) {
                thisFrameInputs.Add(new InputFrame(InputType.P, InputFrameType.PRESSED));
                thisFrameInputs.Add(new InputFrame(InputType.D, InputFrameType.PRESSED));
                thisFrameInputs.Add(new InputFrame(InputType.S, InputFrameType.PRESSED));
                thisFrameInputs.Add(new InputFrame(InputType.HS, InputFrameType.PRESSED));
            }
        }
    }

    public void EnginePreUpdate() {
        if (!input) return;
        
        {
            List<InputFrame> toPush = new();

            if (InputDevicePool.inst.hasGameBaseInput) {
                toPush.AddRange(thisFrameInputs);
                thisFrameInputs.Clear();
            
                // Abstract inputs polled once per Frame and written to the InputBuffer
                foreach (var (actionReference, type) in inputMap) {
                    var action = input.actions.FindAction(actionReference.name);
                    if (action == null) continue;
                
                    if (action.ReadValue<float>() > 0) {
                        toPush.Add(new(type, InputFrameType.HELD));
                    }
                }

                localBuffer.PushAndTick(toPush.ToArray());   
            }
        }
    }
    
}
}
