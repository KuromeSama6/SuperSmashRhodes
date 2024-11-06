using SuperSmashRhodes.FScript.Components;
using UnityEngine;

namespace SuperSmashRhodes.FScript.Instruction {
[FInstruction("mov")]
public class MoveInstruction : FInstruction {
    public MoveInstruction(FLine line) : base(line) {

    }
    protected override void Execute(MoveExecutionContext ctx) {
        RequireMinArgs(2);
    }
}
}
