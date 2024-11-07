using SuperSmashRhodes.FScript;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Instruction;
using UnityEngine;

namespace SuperSmashRhodes.FScript.Instruction {
/// <summary>
/// Interrupt instruction. Skips the number of frames specified by arg 0.
/// </summary>
[FInstruction("int")]
public class InterruptInstruction : FInstruction {
    public int frameCount { get; private set; }

    public InterruptInstruction(FLine line, int addr) : base(line, addr) {
        try {
            frameCount = args[0].IntValue();   
        } catch (ImmediateAccessException e) {
            frameCount = 0;
        }
    }
    
    public override void Execute(FScriptRuntimeContext ctx) {
        RequireMinArgs(1);
        ctx.SetInterrupt(frameCount);
    }
}
}
