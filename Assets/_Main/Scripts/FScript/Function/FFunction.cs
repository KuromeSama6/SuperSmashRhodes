using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SuperSmashRhodes.FScript.Components;
using Unity.VisualScripting;
using UnityEngine;

namespace SuperSmashRhodes.FScript.Function {
public class FFunction {
    public string name { get; private set; }
    private MethodInfo method;
    private bool requiresContext;
    private bool hasParams;
    
    public FFunction(MethodInfo method) {
        this.method = method;
        name = $"{method.DeclaringType.Name}.{method.Name}";

        if (!method.HasAttribute(typeof(FFunctionAttribute)))
            throw new ArgumentException($"FFunction {name}: must be marked with FFunction");
        
        var args = method.GetParameters();
        requiresContext = args.Length > 0 && args[0].ParameterType == typeof(FScriptRuntime);
        
        var firstArg = requiresContext ? 1 : 0;
        
        if (args.Length > 0 && args[firstArg].ParameterType == typeof(FImmediate[])) {
            if (args.Length > 1 + firstArg)
                throw new ArgumentException($"FFunction {name}: FImmediate[] must be the only parameter");
            hasParams = true;
            
        } else {
         
            for (int i = firstArg; i < args.Length; i++) {
                if (args[i].ParameterType != typeof(FImmediate))
                    throw new ArgumentException($"FFunction {name}: paramters must be FImmediate, got {args[i].ParameterType}");
            }   
        }
        
    }

    public FImmediate Call(FScriptRuntime ctx, params FImmediate[] args) {
        List<object> param = new();
        if (requiresContext) param.Add(ctx);
        if (hasParams) param.Add(args);
        else param.AddRange(args);
        
        try {
            var ret = method.Invoke(null, param.ToArray());
            return ret == null ? null : new(ret);
        } catch (Exception e) {
            throw new FunctionInvocationException($"FFunction {name}({string.Join(", ", param)}): Exception thrown during execution", e);
        }
    }
    
}

[AttributeUsage(AttributeTargets.Method)]
public class FFunctionAttribute : Attribute {
    
}

[AttributeUsage(AttributeTargets.Class)]
public class FFunctionLibraryAttribute : Attribute {
    
}

public class FunctionInvocationException : FScriptRuntimeException {
    public FunctionInvocationException(string message) : base(message) { }
    public FunctionInvocationException(string message, Exception innerException) : base(message, innerException) { }
}
}
