using System;
using SuperSmashRhodes.FScript;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Instruction;
using UnityEngine;

namespace SuperSmashRhodes.Battle.Instruction {
[FInstruction("ManagedStateAnimation")]
public class ManagedStateAnimation : FInstruction {
    public ManagedStateAnimation(FLine line, int address) : base(line, address) { }
    public override void Execute(FScriptRuntime ctx) {
        RequireMinArgs(2);
        if (!(ctx.owner is PlayerCharacter player))
            throw new NotImplementedException("Non-player managed animations not implemented yet.");
        
        var name = args[0].StringValue(ctx);
        var length = args[1].IntValue(ctx);
        
        player.GetComponent<CharacterAnimationController>().SetManagedAnimation(name, length);
    }
} 
}
