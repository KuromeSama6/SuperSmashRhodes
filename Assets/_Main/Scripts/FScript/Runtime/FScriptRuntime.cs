using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Sirenix.OdinInspector;
using Spine.Unity;
using SuperSmashRhodes._Main.Scripts.FScript;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Enums;
using SuperSmashRhodes.FScript.Instruction;
using Unity.VisualScripting;
using UnityEngine;

namespace SuperSmashRhodes.FScript {
public class FScriptRuntime : MonoBehaviour {
    [Title("Refrences")]
    public List<FScriptLibrary> scripts = new();
    
    //endregion
    public Dictionary<string, FImmediate> registers { get; } = new();
    public Dictionary<string, FImmediate> variables { get; } = new();
    public Dictionary<string, FImmediate> constants { get; } = new();
    public List<ComparisonFlag> comparsionFlags { get; } = new();
    private Dictionary<string, FInstruction> labels { get; } = new();
    
    public Entity owner { get; private set; }
    public FScriptLinked script { get; private set; }
    public EntityTask mainTask { get; private set; }
    
    private SkeletonAnimation animation => owner.animation;
    
    private void Start() {
        owner = GetComponent<Entity>();
        script = new(owner.mainScript, scripts);
        Init();
    }
    
    private void Init() {
        mainTask = new(this);
        
        // init registers
        foreach (var field in typeof(FScriptRegister).GetFields()) {
            if (!field.IsStatic || field.FieldType != typeof(string)) continue;
            var name = (string)field.GetValue(null);
            var attribute = field.GetCustomAttribute<FScriptTokenInit>();
            registers[name] = new(attribute?.value ?? 0);
        }
        
        // init constants
        foreach (var field in typeof(FScriptConstant).GetFields()) {
            if (!field.IsStatic || field.FieldType != typeof(string)) continue;
            var name = (string)field.GetValue(null);
            var attribute = field.GetCustomAttribute<FScriptTokenInit>();
            constants[name] = new(attribute?.value ?? 0);
        }
        
        // call entityInit
        CallSubroutine($"{script.mainNamespace}::entityInit");
    }

    private void FixedUpdate() {
        // update task
        if (mainTask.isBusy && mainTask.isState) {
            mainTask.TickState();
        }
    }

    public void CallSubroutine(string name, bool interrupt = false) {
        if (interrupt)
            throw new NotImplementedException("Interrupts are not implemented");

        if (!interrupt && mainTask.isBusy)
            throw new FScriptRuntimeException($"Could not call subroutine {name}: task is busy");
        
        if (!script.sections.TryGetValue(name, out var section))
            throw new FScriptRuntimeException($"Calling undefined subroutine {name}");

        mainTask.BeginState(section, false);
        // subroutines are ticked immediately
        if (!interrupt) mainTask.TickState();
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
        throw new NotImplementedException();
    }

    public void SetLabel(LabelInstruction instruction) {
        var name = instruction.labelName;
        if (labels.ContainsKey(name))
            throw new FScriptRuntimeException($"Duplicate label {name}");
        
        labels[name] = instruction;
    }
    
    public void EndState() {
        throw new NotImplementedException();
    }
    
    
    public void WriteRegister(string name, object value) {
        if (!registers.ContainsKey(name))
            throw new FScriptRuntimeException($"Accessing undefined register {name}");
        registers[name].SetValue(value);
    }

    public void WriteConstant(string name, object value) {
        if (!constants.ContainsKey(name))
            throw new FScriptRuntimeException($"Duplicate constant {name}");
        constants[name].SetValue(value);
    }
    
    public int GetLabelAddress(string name) {
        if (!labels.ContainsKey(name))
            throw new FScriptRuntimeException($"Accessing undefined label {name}");
        return labels[name].address;
    }
    
    public void SetFlag(ComparisonFlag flag) {
        if (!comparsionFlags.Contains(flag))
            comparsionFlags.Add(flag);
    }
    
}

public enum ComparisonFlag {
    ZERO,
    SIGN,
    CARRY,
    OVERFLOW,
    PARITY
}

}
