using System;
using System.Collections.Generic;
using NUnit.Framework;
using SuperSmashRhodes.Battle;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SuperSmashRhodes.Input {
public class PlayerInputModule : MonoBehaviour {
    public InputBuffer localBuffer { get; private set; }
    
    private PlayerInput input;
    private PlayerCharacter playerCharacter;
    private List<InputFrame> thisFrameInputs = new();
    
    private void Start() {
        input = GetComponent<PlayerInput>();
        localBuffer = new(120);
        playerCharacter = GetComponent<PlayerCharacter>();
        
    }

    private void Update() {
        
    } 

    private void FixedUpdate() {
        {
            List<InputFrame> toPush = new();
            toPush.AddRange(thisFrameInputs);
            thisFrameInputs.Clear();
            
            // Abstract inputs polled once per Frame and written to the InputBuffer

            // movements
            {
                var action = input.actions.FindAction("Move");
                var value = action.ReadValue<Vector2>().x;
                var facing = playerCharacter.side;
                
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
            // if (PhysicsTickManager.inst.globalFreezeFrames > 0) localBuffer.PushToCurrentFrame(toPush.ToArray());
            // else localBuffer.PushAndTick(toPush.ToArray());
        }
    }
    
    public void OnMove(InputValue input) {
        var value = input.Get<Vector2>().x;
        var facing = playerCharacter.side;
        
        if (!Mathf.Approximately(value, 0)) {
            if (value > 0) thisFrameInputs.Add(new(InputBuffer.TranslateRawDirectionInput(InputType.RAW_MOVE_LEFT, facing), InputFrameType.PRESSED));
            else if (value < 0) thisFrameInputs.Add(new(InputBuffer.TranslateRawDirectionInput(InputType.RAW_MOVE_RIGHT, facing), InputFrameType.PRESSED));
        } 
    }

    public void OnJump(InputValue input) {
        thisFrameInputs.Add(new(InputType.UP, InputFrameType.PRESSED));
    }

    public void OnSlash(InputValue input) {
        thisFrameInputs.Add(new (InputType.S, InputFrameType.PRESSED));
    }

    public void OnHeavySlash(InputValue input) {
        thisFrameInputs.Add(new(InputType.HS, InputFrameType.PRESSED));
    }
    
}
}
