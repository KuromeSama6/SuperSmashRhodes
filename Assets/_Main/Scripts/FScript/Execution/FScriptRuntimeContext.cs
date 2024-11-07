using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Enums;
using SuperSmashRhodes.FScript.Instruction;
using Unity.VisualScripting;
using UnityEngine;

namespace SuperSmashRhodes.FScript {
public class FScriptRuntimeContext {
    //region Hard registers
    public MoveState moveState { get; set; } = MoveState.STARTUP;

    //endregion
    public Dictionary<string, FImmediate> registers { get; } = new();
    public Dictionary<string, FImmediate> variables { get; } = new();
    public Dictionary<string, FImmediate> constants { get; } = new();
    public List<ComparisonFlag> comparsionFlags { get; } = new();
    public EntityState state { get; private set; }
    public Entity owner => state.owner;
    
    private Dictionary<string, FInstruction> labels { get; } = new();
    
    public FScriptRuntimeContext(EntityState state) {
        this.state = state;
        
        // init registers
        foreach (var field in typeof(FScriptRegister).GetFields()) {
            if (!field.IsStatic || field.FieldType != typeof(string)) continue;
            var name = (string)field.GetValue(null);
            var attribute = field.GetCustomAttribute<FScriptTokenInit>();
            InitializeRegister(name, attribute?.value ?? 0);
        }
        
        // init constants
        foreach (var field in typeof(FScriptConstant).GetFields()) {
            if (!field.IsStatic || field.FieldType != typeof(string)) continue;
            var name = (string)field.GetValue(null);
            var attribute = field.GetCustomAttribute<FScriptTokenInit>();
            InitializeRegister(name, attribute?.value ?? 0);
        }
        
        // write character registers
        {
        }
    }

    public FImmediate GetVariableValue(string text) {
        return null;
    }

    public FImmediate GetRegisterValue(string name) {
        if (!registers.ContainsKey(name))
            throw new FScriptRuntimeException($"Accessing undefined register {name}");
        return registers[name];
    }
    
    public FImmediate GetConstantValue(string name) {
        if (!constants.ContainsKey(name))
            throw new FScriptRuntimeException($"Accessing undefined constant {name}");
        return constants[name];
    }

    public void SetInterrupt(int frames) {
        state.AddInterrupt(frames); 
    }

    public void SetLabel(LabelInstruction instruction) {
        var name = instruction.labelName;
        if (labels.ContainsKey(name))
            throw new FScriptRuntimeException($"Duplicate label {name}");
        
        labels[name] = instruction;
    }
    
    public void EndState() {
        state.EndImediately();
    }
    
    
    public void WriteRegister(string name, object value) {
        if (!registers.ContainsKey(name))
            throw new FScriptRuntimeException($"Accessing undefined register {name}");
        registers[name].SetValue(value);
    }

    public void WriteConstant(string name, object value) {
        if (constants.ContainsKey(name))
            throw new FScriptRuntimeException($"Duplicate constant {name}");
        constants[name] = new(value);
    }
    
    public int GetLabelAddress(string name) {
        if (!labels.ContainsKey(name))
            throw new FScriptRuntimeException($"Accessing undefined label {name}");
        return labels[name].address;
    }
    
    public void QueueJump(int address) {
        state.QueueJump(address);
    }
    
    public void SetFlag(ComparisonFlag flag) {
        if (!comparsionFlags.Contains(flag))
            comparsionFlags.Add(flag);
    }
    
    private void InitializeRegister(string name, object value) {
        registers[name] = new(value);
    }
}

public static class FScriptRegister {
    public static readonly string DAMAGE = "dmg";
    public static readonly string HITSTUN = "hit";
    public static readonly string BLOCKSTUN = "block";
    [FScriptTokenInit(GuardType.ALL)]
    public static readonly string GUARD_TYPE = "guard";
    [FScriptTokenInit(CounterHitType.SMALL)]
    public static readonly string COUNTER_TYPE = "ch";
    [FScriptTokenInit(HitState.NONE)]
    public static readonly string HIT_STATE = "hitstate";

    public static readonly string GENERAL_A = "rax";
    public static readonly string GENERAL_B = "rbx";
    public static readonly string GENERAL_C = "rcx";
    public static readonly string GENERAL_D = "rdx";
    public static readonly string SUBROUTINE_RETURN = "rrp";
    public static readonly string CURRENT_INSTRUCTION_PTR = "rci";
    public static readonly string LAST_INSTRUCTION_PTR = "rpi";
    
    public static readonly string CHR_PREJUMP_LEN = "prejumplen";
    public static readonly string CHR_JUMP_LEN = "jumplen";
    public static readonly string CHR_JUMP_VEL = "jumpvel";
}

public static class FScriptConstant {
    [FScriptTokenInit(true)]
    public static readonly string PHY_GROUNDED = "p_grounded";
    [FScriptTokenInit(true)]
    public static readonly string GROUNDED = "l_grounded";
}

public enum ComparisonFlag {
    ZERO,
    SIGN,
    CARRY,
    OVERFLOW,
    PARITY
}

[AttributeUsage(AttributeTargets.Field)]
public class FScriptTokenInit : Attribute {
    public object value { get; }
    public FScriptTokenInit(object value) {
        this.value = value;
    }
}
}
