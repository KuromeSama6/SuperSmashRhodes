using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.Utilities;
using SuperSmashRhodes.FScript;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Instruction;
using UnityEngine;

namespace SuperSmashRhodes {
public static class FInstructionRegistry {
    private static readonly Dictionary<string, Type> registry = new();
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void Init() {
        Scan();
    }
    
    public static void Scan() {
        registry.Clear();
        
        int count = 0;
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
            FInstructionAttribute attr = CustomAttributeExtensions.GetCustomAttribute<FInstructionAttribute>(type);
            if (attr == null) continue;
            
            if (!TypeExtensions.InheritsFrom<FInstruction>(type)) { 
                Debug.LogError($"Class {type.Name} is marked as an FInstruction but does not inherit from FInstruction!");
                continue;
            }

            ++count;
            foreach (var label in attr.labels)
                registry[label] = type;
        }
        
        // Debug.Log($"Scanned {count} FScript instructions");
    }

    public static FInstruction InstantiateInstruction(FLine line, int address) {
        var label = line.instruction;
        if (!registry.ContainsKey(label))
            throw new FScriptRuntimeException($"Unknown instruction {label}: in {line}");
        
        var type = registry[label];
        return (FInstruction) Activator.CreateInstance(type, line, address);
    }
}
}
