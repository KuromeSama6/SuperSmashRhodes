using System;
using System.Collections.Generic;
using SuperSmashRhodes.FScript.Instruction;

namespace SuperSmashRhodes.FScript.Util {
public class AddressRegistry {
    public Dictionary<int, IFScriptAddressable> registry { get; } = new();
    private int counter = 0;

    public int AllocateAddress() {
        return counter++;
    }

    public void RegisterManaged(IFScriptAddressable instruction) {
        if (registry.ContainsKey(instruction.address) || registry.ContainsValue(instruction))
            throw new ArgumentException($"Duplicate instruction or address {instruction.address}");

        registry[instruction.address] = instruction;
    }
    
    public IFScriptAddressable GetInstruction(int address) {
        if (!registry.ContainsKey(address))
            return null;
        return registry[address];
    }

    public void Reset() {
        registry.Clear();
        counter = 0;
    }
}

public interface IFScriptAddressable {
    public int address { get; }
}
}