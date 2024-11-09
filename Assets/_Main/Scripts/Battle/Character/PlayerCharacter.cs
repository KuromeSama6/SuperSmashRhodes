using System;
using System.Linq;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class PlayerCharacter : Entity {
    public float moveDirection { get; private set; }
    public bool isDashing { get; private set; }
    public bool isCrouching { get; private set; }
    public int framesDashed { get; private set; }
    public PlayerInputModule inputModule { get; private set; }
    
    protected override void Start() {
        base.Start();
        inputModule = GetComponent<PlayerInputModule>();
    }

    protected override void Update() {
        base.Update();
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        UpdateInput();
    }

    private void UpdateInput() {
        // get priority sorted list
        var li = (from state in states.Values
            orderby state.inputPriority descending
            select state).ToList();

        foreach (var state in li) {
            if (state == activeState) continue;
            
            if (state.IsInputValid(inputModule.localBuffer) && state.mayEnterState) {
                // check cancel state
                if (activeState.stateData.cancelOptions.Contains(state) || BitUtil.CheckFlag((int)activeState.stateData.cancelFlag, (int)state.type)) {
                    // state is valid
                    BeginState(state);
                    break;
                }
            }
        }
        
    }

    protected override EntityState GetDefaultState() {
        if (!EntityStateRegistry.inst.CreateInstance("CmnNeutral", out var ret, this))
            throw new Exception("Default state [CmnNeutral] not assigned");
        
        return ret;
    }
}
}
