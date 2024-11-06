using System;

namespace SuperSmashRhodes.FScript.Components {
/// <summary>
/// An Immediate is an abstract representation of a value that can be immediately resolved. This may be a literal(constant),
/// a register (starting with a dollar sign($)), or a variable (enclosed in brackets([])).
/// </summary>
public class FImmediate {
    public string value { get; set; }
    public FImmediateType type { get; }
    
    public FImmediate(object value) {
        if (value is FImmediate immediate) {
            this.value = immediate.value;
        } else {
            this.value = value.ToString();
        }
        
        if (this.value.StartsWith("$")) {
            type = FImmediateType.REGISTER;
        } else if (this.value.StartsWith("[") && this.value.EndsWith("]")) {
            type = FImmediateType.VARIABLE;
        } else {
            type = FImmediateType.LITERAL;
        }
    }

    public string StringValue(FScriptRuntimeContext ctx = null) {
        var ret = ResolveValue(ctx);
        return ret;
    }
    
    public int IntValue(FScriptRuntimeContext ctx = null) {
        var ret = ResolveValue(ctx);
        try {
            return int.Parse(ret);
        } catch (Exception e) {
            throw new ImmediateAccessException(e);
        }
    }
    
    public float FloatValue(FScriptRuntimeContext ctx = null) {
        var ret = ResolveValue(ctx);
        try {
            return float.Parse(ret);
        } catch (Exception e) {
            throw new ImmediateAccessException(e);
        }
    }
    
    public bool BoolValue(FScriptRuntimeContext ctx = null) {
        var ret = ResolveValue(ctx);
        try {
            return bool.Parse(ret);
        } catch (Exception e) {
            throw new ImmediateAccessException(e);
        }
    }
    
    public T EnumValue<T>(FScriptRuntimeContext ctx = null) where T : Enum {
        var ret = ResolveValue(ctx);
        try {
            return (T) Enum.Parse(typeof(T), ret);
        } catch (Exception e) {
            throw new ImmediateAccessException(e);
        }
    }

    public void WriteValue(FScriptRuntimeContext ctx, object value) {
        string target = this.value;
        
        // variables are in brackets
        if (target.StartsWith("[") && target.EndsWith("]")) {
            var varName = target.Substring(1, target.Length - 2);
            ctx.variables[varName] = new(value);
        } else {
            var regName = target;
            ctx.registers[regName] = new(value);
        }
    }
    
    private string ResolveValue(FScriptRuntimeContext ctx = null) {
        if (ctx == null)
            return value;
        
        // registers starts with $
        if (value.StartsWith("$")) {
            var regName = value.Substring(1);
            var ret = ctx.GetRegisterValue(regName);

            if (ret == null)
                throw new ImmediateAccessException($"Accessing uninitiated register {regName}");
            return ret.ResolveValue();
        }
        
        // variables are in brackets
        if (value.StartsWith("[") && value.EndsWith("]")) {
            var varName = value.Substring(1, value.Length - 2);
            var ret = ctx.GetVariableValue(varName);

            if (ret == null)
                throw new ImmediateAccessException($"Accessing undeclared variable {varName}");
            return ret.ResolveValue();
        }

        return value;
    }

    public override string ToString() {
        return $"FImmediate({value})";
    }
}

public enum FImmediateType {
    LITERAL,
    REGISTER,
    VARIABLE
}
}
