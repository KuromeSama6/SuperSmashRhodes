using System;
using System.Collections.Generic;
using System.Reflection;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Framework {
public abstract class NamedTokenRegistry<TRegistry, TTarget> : GlobalSingleton<TRegistry> where TRegistry: new() {
    public static Dictionary<string, Type> registry { get; private set; } = new();

    public NamedTokenRegistry() {
        Scan();
    }
    
    protected void Scan() {
        registry.Clear();
        var targetType = typeof(TTarget);
        
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
            NamedTokenAttribute attr = type.GetCustomAttribute<NamedTokenAttribute>();
            if (attr == null) continue;
            if (targetType.IsAssignableFrom(type)) {
                if (registry.ContainsKey(attr.name))
                    throw new Exception($"TokenRegistry {GetType()}: duplicate token {attr.name}");
                
                registry[attr.name] = type;
                
                // Debug.Log($"TokenRegistry {GetType()}: scanned {attr.name}");
            }
        }
    }
    
    public bool CreateInstance(string name, out TTarget ret, params object[] args) {
        if (registry.TryGetValue(name, out var type)) {
            ret = CreateTargetInstance(type, args);
            return true;
        }
        ret = default;
        return false;
    }

    protected virtual TTarget CreateTargetInstance(Type type, params object[] args) {
        return (TTarget)Activator.CreateInstance(type, args);
    }
    
}

public abstract class NamedToken {
    public string id { get; private set; }
    public NamedToken() {
        var attr = GetType().GetCustomAttribute<NamedTokenAttribute>();
        if (attr == null) 
            throw new Exception($"NamedToken {GetType()}: missing NamedTokenAttribute");
        
        id = attr.name;
    }
}

}
