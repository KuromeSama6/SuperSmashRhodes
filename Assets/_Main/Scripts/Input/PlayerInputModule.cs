using System;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.FScript.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SuperSmashRhodes.Input {
public class PlayerInputModule : MonoBehaviour {
    public InputBuffer localBuffer { get; private set; }
    
    private PlayerInput input;
    private PlayerCharacter playerCharacter;
    
    private void Start() {
        input = GetComponent<PlayerInput>();
        localBuffer = new(30);
        playerCharacter = GetComponent<PlayerCharacter>();
        
    }

    private void Update() {
        
    } 

    private void FixedUpdate() {
        {
            List<InputType> toPush = new();
            
            // Abstract inputs polled once per Frame and written to the InputBuffer

            // movements
            {
                var action = input.actions.FindAction("Move");
                var value = action.ReadValue<Vector2>().x;
                var facing = playerCharacter.facing;
                
                if (!Mathf.Approximately(value, 0)) {
                    if (value > 0) toPush.Add(InputBuffer.TranslateRawDirectionInput(InputType.RAW_MOVE_LEFT, facing));
                    else if (value < 0) toPush.Add(InputBuffer.TranslateRawDirectionInput(InputType.RAW_MOVE_RIGHT, facing));
                } 
            }

            localBuffer.PushAndTick(toPush.ToArray());
        }
    }
}
}
