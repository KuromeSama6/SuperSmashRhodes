using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Instruction;
using SuperSmashRhodes.FScript.Util;
using UnityEngine;

namespace SuperSmashRhodes.FScript {
public abstract class FScriptProcedure : FScriptBlockImplementation, IFScriptAddressable {
    public List<FInstruction> ops { get; } = new();
    public int totalFrames { get; }
    public int address { get; private set; }
    public override string invalidMessage => null;
    
    public FScriptProcedure(FBlock block, AddressRegistry registry) : base(block) {
        address = registry.AllocateAddress();
        registry.RegisterManaged(this);
        
        foreach (var line in block.lines) {
            int addr = registry.AllocateAddress();
            var op = FInstructionRegistry.InstantiateInstruction(line, addr);
            registry.RegisterManaged(op);
            
            ops.Add(op);
        }

        totalFrames = ops.FindAll(c => c is InterruptInstruction)
            .Select(c => (InterruptInstruction)c)
            .Sum(c => c.frameCount);
    }
    
    public int CountFramesAfter(FInstruction instruction) {
        var index = ops.IndexOf(instruction);
        if (index == -1) return -1;
        
        for (int i = index; i < ops.Count; i++) {
            var op = ops[i];
            if (op is InterruptInstruction interrupt) return interrupt.frameCount;
        }
        return 0;
    }

    public int CountFramesAfter<T>() where T : FInstruction {
        var op = ops.Find(o => o is T);
        if (op == null) return -1;
        return CountFramesAfter(op);
    }

    public void ProcedureInit(FScriptRuntimeContext ctx) {
        foreach (var op in ops) {
            if (op is LabelInstruction label) {
                ctx.SetLabel(label);
            }
        }
    }

    public override string ToString() {
        return $"FScriptProcedure({block.label})";
    }
}
}
