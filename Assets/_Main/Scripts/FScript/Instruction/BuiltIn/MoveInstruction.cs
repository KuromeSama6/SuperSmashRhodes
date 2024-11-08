using SuperSmashRhodes.FScript.Components;
using UnityEngine;

namespace SuperSmashRhodes.FScript.Instruction {
[FInstruction("mov")]
public class MoveInstruction : FInstruction {
    public MoveInstruction(FLine line, int addr) : base(line, addr) {

    }
    
    public override void Execute(FScriptRuntime ctx) {
        RequireMinArgs(2);
        args[0].WriteValue(ctx, args[1]);
    }
}
}
