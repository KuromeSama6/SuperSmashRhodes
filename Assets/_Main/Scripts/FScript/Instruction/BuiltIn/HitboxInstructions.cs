using SuperSmashRhodes.FScript.Components;

namespace SuperSmashRhodes.FScript.Instruction {
[FInstruction("hitbox")]
public class HitboxInstruction : FInstruction {
    public HitboxInstruction(FLine line) : base(line) { }
    protected override void Execute(MoveExecutionContext ctx) {
        
    }
}

[FInstruction("hurtbox")]
public class HurtboxInstruction : FInstruction {
    public HurtboxInstruction(FLine line) : base(line) { }
    protected override void Execute(MoveExecutionContext ctx) { }
}
}
