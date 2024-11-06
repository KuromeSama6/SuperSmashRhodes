using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Enums;

namespace SuperSmashRhodes.FScript.Instruction {
[FInstruction("startup")]
public class MoveStartupInstruction : FInstruction {
    public MoveStartupInstruction(FLine line) : base(line) { }
    protected override void Execute(FScriptRuntimeContext ctx) {
        ctx.moveState = MoveState.STARTUP;
        ctx.WriteRegister(FScriptRegister.HIT_STATE, HitState.COUNTER);
    }
}

[FInstruction("active")]
public class MoveActiveInstruction : FInstruction {
    public MoveActiveInstruction(FLine line) : base(line) { }
    protected override void Execute(FScriptRuntimeContext ctx) {
        ctx.moveState = MoveState.ACTIVE;
        ctx.WriteRegister(FScriptRegister.HIT_STATE, HitState.COUNTER);
    }
}

[FInstruction("recovery")]
public class MoveRecoveryInstruction : FInstruction {
    public MoveRecoveryInstruction(FLine line) : base(line) { }
    protected override void Execute(FScriptRuntimeContext ctx) {
        ctx.moveState = MoveState.RECOVERY;
        ctx.WriteRegister(FScriptRegister.HIT_STATE, HitState.PUNISH);
    }
}

[FInstruction("endm")]
public class MoveEndInstruction : FInstruction {
    public MoveEndInstruction(FLine line) : base(line) { }
    protected override void Execute(FScriptRuntimeContext ctx) {
        ctx.WriteRegister(FScriptRegister.HIT_STATE, HitState.NONE);
    }
}
}