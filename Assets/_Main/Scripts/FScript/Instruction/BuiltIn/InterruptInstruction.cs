using SuperSmashRhodes.FScript;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Instruction;

namespace SuperSmashRhodes.FScript.Instruction {
/// <summary>
/// Interrupt instruction. Skips the number of frames specified by arg 0.
/// </summary>
[FInstruction("int")]
public class InterruptInstruction : FInstruction {
    public int frameCount { get; private set; }

    public InterruptInstruction(FLine line) : base(line) {
        frameCount = args[0].IntValue();
    }
    
    protected override void Execute(MoveExecutionContext ctx) {
        RequireMinArgs(1);
        ctx.SetInterrupt(frameCount);
    }
}
}
