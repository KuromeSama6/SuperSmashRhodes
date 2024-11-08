using System;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Function;

namespace SuperSmashRhodes.FScript.Instruction {
[FInstruction("call")]
public class FunctionCallInstruction : FInstruction{
    public FunctionCallInstruction(FLine line, int address) : base(line, address) { }
    public override void Execute(FScriptRuntime ctx) {
        RequireMinArgs(1);
        string functionName = args[0].StringValue();

        try {
            FFunctionRegistry.CallFunction(functionName, ctx, args[1..]);
            
        } catch (ArgumentException) {
            throw new FScriptRuntimeException($"Function {functionName} not found.");
        }
        
    }
}
}
