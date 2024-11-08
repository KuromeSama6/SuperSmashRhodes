using SuperSmashRhodes.FScript.Components;

namespace SuperSmashRhodes.FScript.Instruction {

public abstract class ArithmeticInstruction : FInstruction {
    public ArithmeticInstruction(FLine line, int address) : base(line, address) { }
    public override void Execute(FScriptRuntime ctx) {
        RequireMinArgs(2);
        var a = args[0].FloatValue(ctx, true);
        var b = args[1].FloatValue(ctx);
        var result = Calculate(a, b);

        args[0].WriteValue(ctx, result);
    }
    
    protected abstract float Calculate(float a, float b);
}

[FInstruction("add")]
public class AddInstruction : ArithmeticInstruction {
    public AddInstruction(FLine line, int address) : base(line, address) { }
    protected override float Calculate(float a, float b) {
        return a + b;
    }
}

[FInstruction("sub")]
public class SubtractInstruction : ArithmeticInstruction {
    public SubtractInstruction(FLine line, int address) : base(line, address) { }
    protected override float Calculate(float a, float b) {
        return a - b;
    }
}

[FInstruction("mul")]
public class MultiplyInstruction : ArithmeticInstruction {
    public MultiplyInstruction(FLine line, int address) : base(line, address) { }
    protected override float Calculate(float a, float b) {
        return a * b;
    }
}

[FInstruction("div")]
public class DivideInstruction : ArithmeticInstruction {
    public DivideInstruction(FLine line, int address) : base(line, address) { }
    protected override float Calculate(float a, float b) {
        return a / b;
    }
}

}
