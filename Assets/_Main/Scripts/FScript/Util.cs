using System;
using System.Collections.Generic;
using SuperSmashRhodes.FScript.Instruction;

namespace SuperSmashRhodes.FScript.Util {
public class AddressRegistry {
    public Dictionary<int, IFScriptToken> registry { get; } = new();
    private int counter = 0x1000;

    public int AllocateAddress() {
        return counter++;
    }

    public void RegisterManaged(IFScriptToken instruction) {
        if (registry.ContainsKey(instruction.address) || registry.ContainsValue(instruction))
            throw new ArgumentException($"Duplicate instruction or address {instruction.address}");

        registry[instruction.address] = instruction;
    }
    
    public IFScriptToken GetInstruction(int address) {
        if (!registry.ContainsKey(address))
            return null;
        return registry[address];
    }

    public void Reset() {
        registry.Clear();
        counter = 0;
    }
}

public interface IFScriptToken {
    public int address { get; }
}
}