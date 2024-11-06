using SuperSmashRhodes.FScript;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Instruction;

namespace SuperSmashRhodes.FScript.Instruction {
/// <summary>
/// No-operation instruction.
/// </summary>
[FInstruction("anim")]
public class AnimationInstruction : FInstruction {
    public AnimationInstruction(FLine line) : base(line) {
        
    }
    protected override void Execute(MoveExecutionContext ctx) {
        // NO-OP
    }
}

}
