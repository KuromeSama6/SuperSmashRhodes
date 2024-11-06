using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.Utilities;
using SuperSmashRhodes.FScript.Components;
using UnityEngine;

namespace SuperSmashRhodes.FScript.Instruction {
public abstract class FInstruction {
    public FImmediate[] args { get; private set; }
    
    public FInstruction(FLine line) {
        args = line.args;
    }
    protected abstract void Execute(MoveExecutionContext ctx);
    
    protected void RequireMinArgs(int count) {
        if (args.Length < count) 
            throw new InstructionException($"Expected at least {count} arguments, got {args.Length}");
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class FInstructionAttribute : Attribute {
    public string label { get; private set; }
    public FInstructionAttribute(string label) {
        this.label = label;
    }
    
}
}
