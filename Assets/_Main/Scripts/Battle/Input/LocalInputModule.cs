using System;
using System.Collections.Generic;
using NUnit.Framework;
using SuperSmashRhodes.Battle;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SuperSmashRhodes.Input {
[RequireComponent(typeof(PlayerInput))]
public class LocalInputModule : MonoBehaviour, IInputProvider {
    public InputBuffer inputBuffer => localBuffer;

    private PlayerInput input;
    private List<InputFrame> thisFrameInputs = new();
    private InputBuffer localBuffer;
    
    private void Start() {
        input = GetComponent<PlayerInput>();
        localBuffer = new(120);
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
                
                if (!Mathf.Approximately(value, 0)) {
                    if (value > 0) toPush.Add(new(InputType.RAW_MOVE_LEFT, InputFrameType.HELD));
                    else if (value < 0) toPush.Add(new(InputType.RAW_MOVE_RIGHT, InputFrameType.HELD));
                } 
                
            }
            
            // singles
            {
                if (input.actions.FindAction("Dash").ReadValue<float>() > 0f) toPush.Add(new(InputType.DASH, InputFrameType.HELD));
                if (input.actions.FindAction("Crouch").ReadValue<float>() > 0f) toPush.Add(new(InputType.DOWN, InputFrameType.HELD));
                if (input.actions.FindAction("Jump").ReadValue<float>() > 0f) toPush.Add(new(InputType.UP, InputFrameType.HELD));
                
                if (input.actions.FindAction("Drive").ReadValue<float>() > 0f) toPush.Add(new(InputType.D, InputFrameType.HELD));
                if (input.actions.FindAction("HeavySlash").ReadValue<float>() > 0f) toPush.Add(new(InputType.HS, InputFrameType.HELD));
                if (input.actions.FindAction("Slash").ReadValue<float>() > 0f) toPush.Add(new(InputType.S, InputFrameType.HELD));
                if (input.actions.FindAction("Punch").ReadValue<float>() > 0f) toPush.Add(new(InputType.P, InputFrameType.HELD));
            }

            localBuffer.PushAndTick(toPush.ToArray());
            // if (PhysicsTickManager.inst.globalFreezeFrames > 0) localBuffer.PushToCurrentFrame(toPush.ToArray());
            // else localBuffer.PushAndTick(toPush.ToArray());
        }
    }
    
    public void OnMove(InputValue input) {
        var value = input.Get<Vector2>().x;
        
        if (!Mathf.Approximately(value, 0)) {
            if (value > 0) thisFrameInputs.Add(new(InputType.RAW_MOVE_LEFT, InputFrameType.PRESSED));
            else if (value < 0) thisFrameInputs.Add(new(InputType.RAW_MOVE_RIGHT, InputFrameType.PRESSED));
        } 
    }

    public void OnDash(InputValue input) {
        thisFrameInputs.Add(new(InputType.DASH, InputFrameType.PRESSED));
    }
    
    public void OnCrouch(InputValue input) {
        thisFrameInputs.Add(new(InputType.DOWN, InputFrameType.PRESSED));
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

    public void OnDrive(InputValue input) {
        thisFrameInputs.Add(new(InputType.D, InputFrameType.PRESSED));
    }

    public void OnPunch(InputValue input) {
        thisFrameInputs.Add(new(InputType.P, InputFrameType.PRESSED));
    }
    
    public void OnForceReset(InputValue input) {
        thisFrameInputs.Add(new(InputType.FORCE_RESET, InputFrameType.PRESSED));
    }

    public void OnBurst(InputValue input) {
        // Debug.Log("burst");
        thisFrameInputs.Add(new InputFrame(InputType.P, InputFrameType.PRESSED));
        thisFrameInputs.Add(new InputFrame(InputType.D, InputFrameType.PRESSED));
        thisFrameInputs.Add(new InputFrame(InputType.S, InputFrameType.PRESSED));
        thisFrameInputs.Add(new InputFrame(InputType.HS, InputFrameType.PRESSED));
    }
    
}
}
