using System;
using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Input;

namespace SuperSmashRhodes.FScript {
public class FScriptInputMethod : FScriptBlockImplementation {
    public List<InputChord> chords = new();
    public FScriptInputMethod(FBlock block) : base(block) {
        foreach (var line in block.lines) {
            List<InputType> inputs = new();
            
            foreach (var arg in line.args) {
                string value = arg.StringValue();
                
                if (!Enum.TryParse(value.ToUpper(), out InputType input)) {
                    throw new FScriptException($"Unknown input type: {value}");
                }
                    
                inputs.Add(input);
            }
            
            chords.Add(new InputChord(inputs.ToArray()));
        }
    }
    
    public override string invalidMessage {
        get {
            if (chords.Count == 0) return "Move must have at least one input";
            return null;
        }
    }
}

public class InputChord {
    public InputType[] inputs;

    public InputChord(params InputType[] inputs) {
        this.inputs = inputs;
    }

    public bool HasInput(InputType type) {
        return inputs.Contains(type);
    }

    public override string ToString() {
        return $"InputChord({string.Join(", ", inputs)})";
    }
}
}
