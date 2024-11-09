using System;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
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
            List<InputFrame> toPush = new();
            
            // Abstract inputs polled once per Frame and written to the InputBuffer

            // movements
            {
                var action = input.actions.FindAction("Move");
                var value = action.ReadValue<Vector2>().x;
                var facing = playerCharacter.facing;
                
                if (!Mathf.Approximately(value, 0)) {
                    if (value > 0) toPush.Add(new(InputBuffer.TranslateRawDirectionInput(InputType.RAW_MOVE_LEFT, facing), InputFrameType.HELD));
                    else if (value < 0) toPush.Add(new(InputBuffer.TranslateRawDirectionInput(InputType.RAW_MOVE_RIGHT, facing), InputFrameType.HELD));
                } 
                
            }
            
            // singles
            {
                if (input.actions.FindAction("Dash").ReadValue<float>() > 0f) toPush.Add(new(InputType.DASH, InputFrameType.HELD));
                if (input.actions.FindAction("Crouch").ReadValue<float>() > 0f) toPush.Add(new(InputType.DOWN, InputFrameType.HELD));
            }

            localBuffer.PushAndTick(toPush.ToArray());
        }
    }
}
}
