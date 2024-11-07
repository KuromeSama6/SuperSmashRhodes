using SuperSmashRhodes.FScript.Components;
using UnityEngine;

namespace SuperSmashRhodes.FScript.Instruction {
[FInstruction("cmp")]
public class CompareInstruction : FInstruction {
    public CompareInstruction(FLine line, int address) : base(line, address) { }
    public override void Execute(FScriptRuntimeContext ctx) {
        RequireMinArgs(2);
        var a = args[0];
        var b = args[1];
        var aString = a.StringValue(ctx);
        var bString = b.StringValue(ctx);
        
        ctx.comparsionFlags.Clear();
        
        // equal
        if (aString == bString)
            ctx.SetFlag(ComparisonFlag.ZERO);
        
        // numeric
        if (float.TryParse(aString, out _) && float.TryParse(bString, out _)) {
            var aVal = a.FloatValue(ctx);
            var bVal = b.FloatValue(ctx);
            
            if (Mathf.Approximately(aVal, bVal)) {
                ctx.SetFlag(ComparisonFlag.ZERO);
            }
            if (aVal < bVal) {
                ctx.SetFlag(ComparisonFlag.SIGN);
            }
        }
    }
}
}
