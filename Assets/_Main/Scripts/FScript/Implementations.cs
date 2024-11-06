using SuperSmashRhodes.FScript.Components;

namespace SuperSmashRhodes.FScript {
public abstract class FScriptBlockImplementation {
    public FBlock block { get; private set; }
    public FScriptBlockImplementation(FBlock block) {
        this.block = block;
    }
    
    public abstract string invalidMessage { get; }
}
}
