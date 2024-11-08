using SuperSmashRhodes.FScript;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Instruction;

namespace SuperSmashRhodes.FScript.Instruction {
/// <summary>
/// No-operation instruction.
/// </summary>
[FInstruction("nop")]
public class NOPInstruction : FInstruction {
    public NOPInstruction(FLine line, int addr) : base(line, addr) { }
    public override void Execute(FScriptRuntime ctx) {
        // NO-OP
    }
}

[FInstruction("label")]
public class LabelInstruction : FInstruction {
    public string labelName { get; private set; }
    
    public LabelInstruction(FLine line, int addr) : base(line, addr) {
        RequireMinArgs(1);
        labelName = args[0].StringValue().TrimEnd(':');
    }
    
    public override void Execute(FScriptRuntime ctx) {
        RequireMinArgs(1);
    }
}

}
