using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.Utilities;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Util;
using UnityEngine;

namespace SuperSmashRhodes.FScript.Instruction {
public abstract class FInstruction : IFScriptAddressable {
    public FImmediate[] args { get; private set; }
    public FLine rawLine { get; private set; }
    public int address { get; private set; }
    
    public FInstruction(FLine line, int address) {
        rawLine = line;
        args = line.args;
        this.address = address;
    }
    
    public abstract void Execute(FScriptRuntimeContext ctx);
    
    protected void RequireMinArgs(int count) {
        if (args.Length < count) 
            throw new InstructionException($"Expected at least {count} arguments, got {args.Length}");
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class FInstructionAttribute : Attribute {
    public string[] labels { get; private set; }
    public FInstructionAttribute(params string[] labels) {
        this.labels = labels;
    }
    
}
}
