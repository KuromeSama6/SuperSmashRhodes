using System;
using System.Collections.Generic;
using System.Reflection;
using SuperSmashRhodes.FScript.Components;
using UnityEngine;

namespace SuperSmashRhodes.FScript.Function {
public static class FFunctionRegistry {
    private static readonly Dictionary<string, FFunction> registry = new();
        
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void Init() {
        registry.Clear();
        
        int count = 0;
        int skipped = 0;
        
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
            FFunctionLibraryAttribute libraryAttribute = type.GetCustomAttribute<FFunctionLibraryAttribute>();
            if (libraryAttribute == null) continue;
            
            // scan methods
            foreach (var method in type.GetMethods()) {
                if (method.GetCustomAttribute<FFunctionAttribute>() == null) continue;

                try {
                    FFunction func = new(method);
                    if (registry.ContainsKey(func.name)) {
                        Debug.LogError($"Function {func.name} is already registered in registry. Skipping.");
                        ++skipped;
                        continue;
                    }
                    
                    registry[func.name] = func;
                    ++count;
                    
                } catch (ArgumentException e) {
                    Debug.LogError($"An error occurred while registering method {method.Name} in class {type.Name}: {e.Message}");
                    ++skipped;
                }
            }
        }
        
        Debug.Log($"FFunction Registry registered {count} functions, skipped {skipped}");
    }

    public static FImmediate CallFunction(string name, FScriptRuntimeContext ctx, params FImmediate[] args) {
        if (!registry.TryGetValue(name, out var func))
            throw new ArgumentException($"Calling non-existent function {name}");

        var ret = func.Call(ctx, args);
        if (ret != null) ctx.WriteRegister(FScriptRegister.GENERAL_A, ret);
        return ret;
    }
}
}
