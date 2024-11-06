using System;
using System.Collections.Generic;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Enums;

namespace SuperSmashRhodes.FScript {
public class FScriptRuntimeContext {
    //region Hard registers
    public MoveState moveState { get; set; } = MoveState.STARTUP;

    //endregion
    public Dictionary<string, FImmediate> registers { get; } = new();
    public Dictionary<string, FImmediate> variables { get; } = new();

    public FScriptRuntimeContext() {
        // registers
        InitializeRegister(FScriptRegister.DAMAGE, 0); // damage
        InitializeRegister(FScriptRegister.HITSTUN, 0); // hitstun
        InitializeRegister(FScriptRegister.BLOCKSTUN, 0); // blockstun
        InitializeRegister(FScriptRegister.GUARD_TYPE, GuardType.ALL); // guard type
        InitializeRegister(FScriptRegister.COUNTER_TYPE, CounterHitType.SMALL); // counter-hit type
        InitializeRegister(FScriptRegister.HIT_STATE, HitState.NONE);
        InitializeRegister(FScriptRegister.GENERAL_A, 0);
        InitializeRegister(FScriptRegister.GENERAL_B, 0);
        InitializeRegister(FScriptRegister.GENERAL_C, 0);
        InitializeRegister(FScriptRegister.GENERAL_D, 0);
    }

    public FImmediate GetVariableValue(string text) {
        return null;
    }

    public FImmediate GetRegisterValue(string text) {
        return null;
    }

    public void SetInterrupt(int frames) {
        //TODO: Interrupt   
    }

    public void WriteRegister(string name, object value) {
        if (!registers.ContainsKey(name))
            throw new FScriptRuntimeException($"Accessing undefined register {name}");
        registers[name] = new FImmediate(value);
    }
    
    private void InitializeRegister(string name, object value) {
        registers[name] = new(value);
    }
}

public static class FScriptRegister {
    public static readonly string DAMAGE = "dmg";
    public static readonly string HITSTUN = "hit";
    public static readonly string BLOCKSTUN = "block";
    public static readonly string GUARD_TYPE = "guard";
    public static readonly string COUNTER_TYPE = "ch";
    public static readonly string HIT_STATE = "hitstate";

    public static readonly string GENERAL_A = "eax";
    public static readonly string GENERAL_B = "ebx";
    public static readonly string GENERAL_C = "ecx";
    public static readonly string GENERAL_D = "edx";
}
}
