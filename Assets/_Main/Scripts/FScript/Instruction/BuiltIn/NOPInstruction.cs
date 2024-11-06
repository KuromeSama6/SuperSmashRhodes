using SuperSmashRhodes.FScript;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Instruction;

namespace SuperSmashRhodes.FScript.Instruction {
/// <summary>
/// No-operation instruction.
/// </summary>
[FInstruction("nop")]
public class NOPInstruction : FInstruction {
    public NOPInstruction(FLine line) : base(line) {
        
    }
    protected override void Execute(FScriptRuntimeContext ctx) {
        // NO-OP
    }
}

}
