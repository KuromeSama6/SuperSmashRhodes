using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Instruction;
using UnityEngine;

namespace SuperSmashRhodes.FScript {
public abstract class FScriptProcedure : FScriptBlockImplementation {
    public List<FInstruction> ops { get; } = new();
    public int totalFrames { get; }
    public override string invalidMessage => null;
    
    public FScriptProcedure(FBlock block) : base(block) {
        foreach (var line in block.lines) {
            var op = FInstructionRegistry.InstantiateInstruction(line);
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
}
}
