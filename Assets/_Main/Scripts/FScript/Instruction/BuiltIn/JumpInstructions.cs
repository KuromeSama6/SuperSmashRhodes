using System.Collections.Generic;
using SuperSmashRhodes.FScript.Components;

namespace SuperSmashRhodes.FScript.Instruction {
[FInstruction("jmp")]
public class JumpInstruction : FInstruction {
    public JumpInstruction(FLine line, int address) : base(line, address) { }
    public override void Execute(FScriptRuntimeContext ctx) {
        RequireMinArgs(1);
        var destination = args[0];
        int addr = destination.IntValue(ctx);
        
        if (ShouldJump(new(ctx.comparsionFlags)))
            ctx.QueueJump(addr);
    }

    public virtual bool ShouldJump(HashSet<ComparisonFlag> flags) {
        return true;
    }
}

// Jump if Equal
[FInstruction("je", "jz")]
public class JumpIfEqualInstruction : JumpInstruction {
    public JumpIfEqualInstruction(FLine line, int address) : base(line, address) { }
    public override bool ShouldJump(HashSet<ComparisonFlag> flags) {
        return flags.Contains(ComparisonFlag.ZERO);
    }
}

// Jump if Not Equal (JNE / JNZ)
[FInstruction("jne", "jnz")]
public class JumpIfNotEqualInstruction : JumpInstruction {
    public JumpIfNotEqualInstruction(FLine line, int address) : base(line, address) { }
    public override bool ShouldJump(HashSet<ComparisonFlag> flags) {
        return !flags.Contains(ComparisonFlag.ZERO);
    }
}

// Jump if Greater (JG)
[FInstruction("jg")]
public class JumpIfGreaterInstruction : JumpInstruction {
    public JumpIfGreaterInstruction(FLine line, int address) : base(line, address) { }
    public override bool ShouldJump(HashSet<ComparisonFlag> flags) {
        return !flags.Contains(ComparisonFlag.SIGN) && !flags.Contains(ComparisonFlag.ZERO);
    }
}

// Jump if Greater or Equal (JGE)
[FInstruction("jge")]
public class JumpIfGreaterOrEqualInstruction : JumpInstruction {
    public JumpIfGreaterOrEqualInstruction(FLine line, int address) : base(line, address) { }
    public override bool ShouldJump(HashSet<ComparisonFlag> flags) {
        return !flags.Contains(ComparisonFlag.SIGN) || flags.Contains(ComparisonFlag.ZERO);
    }
}

// Jump if Less (JL)
[FInstruction("jl")]
public class JumpIfLessInstruction : JumpInstruction {
    public JumpIfLessInstruction(FLine line, int address) : base(line, address) { }
    public override bool ShouldJump(HashSet<ComparisonFlag> flags) {
        return flags.Contains(ComparisonFlag.SIGN) && !flags.Contains(ComparisonFlag.ZERO);
    }
}

// Jump if Less or Equal (JLE)
[FInstruction("jle")]
public class JumpIfLessOrEqualInstruction : JumpInstruction {
    public JumpIfLessOrEqualInstruction(FLine line, int address) : base(line, address) { }
    public override bool ShouldJump(HashSet<ComparisonFlag> flags) {
        return flags.Contains(ComparisonFlag.SIGN) || flags.Contains(ComparisonFlag.ZERO);
    }
}

// Jump if Overflow (JO)
[FInstruction("jo")]
public class JumpIfOverflowInstruction : JumpInstruction {
    public JumpIfOverflowInstruction(FLine line, int address) : base(line, address) { }
    public override bool ShouldJump(HashSet<ComparisonFlag> flags) {
        return flags.Contains(ComparisonFlag.OVERFLOW);
    }
}

// Jump if Not Overflow (JNO)
[FInstruction("jno")]
public class JumpIfNotOverflowInstruction : JumpInstruction {
    public JumpIfNotOverflowInstruction(FLine line, int address) : base(line, address) { }
    public override bool ShouldJump(HashSet<ComparisonFlag> flags) {
        return !flags.Contains(ComparisonFlag.OVERFLOW);
    }
}

// Jump if Carry (JC / JB)
[FInstruction("jc", "jb")]
public class JumpIfCarryInstruction : JumpInstruction {
    public JumpIfCarryInstruction(FLine line, int address) : base(line, address) { }
    public override bool ShouldJump(HashSet<ComparisonFlag> flags) {
        return flags.Contains(ComparisonFlag.CARRY);
    }
}

// Jump if Not Carry (JNC / JAE)
[FInstruction("jnc", "jae")]
public class JumpIfNotCarryInstruction : JumpInstruction {
    public JumpIfNotCarryInstruction(FLine line, int address) : base(line, address) { }
    public override bool ShouldJump(HashSet<ComparisonFlag> flags) {
        return !flags.Contains(ComparisonFlag.CARRY);
    }
}
}
