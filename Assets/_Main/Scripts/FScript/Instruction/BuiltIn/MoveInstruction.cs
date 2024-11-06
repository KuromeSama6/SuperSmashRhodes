using SuperSmashRhodes.FScript.Components;
using UnityEngine;

namespace SuperSmashRhodes.FScript.Instruction {
[FInstruction("mov")]
public class MoveInstruction : FInstruction {
    public MoveInstruction(FLine line) : base(line) {

    }
    
    protected override void Execute(FScriptRuntimeContext ctx) {
        RequireMinArgs(2);
        args[0].WriteValue(ctx, args[1]);
    }
}
}
