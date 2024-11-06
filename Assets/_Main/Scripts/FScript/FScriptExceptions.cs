using System;

namespace SuperSmashRhodes.FScript {
public class FScriptException : Exception {
    public FScriptException(string message) : base(message) {
        
    }
    public FScriptException(string message, Exception innerException) : base(message, innerException) {
        
    }
}

public class FScriptRuntimeException : FScriptException {
    public FScriptRuntimeException(string message) : base(message) {
        
    }
    public FScriptRuntimeException(string message, Exception innerException) : base(message, innerException) {
        
    }
}

public class InstructionException : FScriptRuntimeException {
    public InstructionException(string message) : base(message) {
        
    }
}

public class ImmediateAccessException : FScriptRuntimeException {
    public ImmediateAccessException(string message) : base(message) {
        
    }
    
    public ImmediateAccessException(Exception innerException) : base("Error accessing variable", innerException) {
        
    }
}

}
