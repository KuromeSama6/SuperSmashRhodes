using SuperSmashRhodes.FScript;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Instruction;
using UnityEngine;

namespace SuperSmashRhodes.Battle.Instruction {
[FInstruction("SetForcedAirborneFrames")]
public class SetForcedAirborneFrames : FInstruction {
    public SetForcedAirborneFrames(FLine line, int address) : base(line, address) { }
    public override void Execute(FScriptRuntime ctx) {
        RequireMinArgs(1);
        ctx.owner.forcedAirborneFrames = args[0].IntValue();
    }
} 

[FInstruction("SetAirborneState")]
public class SetAirborneState : FInstruction {
    public SetAirborneState(FLine line, int address) : base(line, address) { }
    public override void Execute(FScriptRuntime ctx) {
        RequireMinArgs(1);
        ctx.owner.isLogicallyGrounded = !args[0].BoolValue();
    }
}
}
