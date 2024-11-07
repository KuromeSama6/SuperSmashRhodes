using System;
using System.Reflection;
using SuperSmashRhodes.FScript.Components;
using UnityEngine;

namespace SuperSmashRhodes.FScript {
public class FScriptDescriptor : FScriptBlockImplementation {
    public string id { get; private set; }
    public string prettyName { get; private set; }
    public int bufferFrames { get; private set; }
    public int priority { get; private set; } = -1;
    public bool followUp { get; private set; } = false;

    public FScriptDescriptor(FBlock block) : base(block){
        if (block.label != "data")
            throw new FScriptException("FScript descriptor block must be labeled .data");

        foreach (FLine line in block.lines) {
            var key = line.instruction;
            if (line.args.Length == 0)
                throw new FScriptException($"Descriptor property {key} missing value.");
            
            var value = line.args[0];
            
            // set properties
            PropertyInfo field = GetType().GetProperty(key);
            if (field == null) {
                Debug.LogWarning($"Script .data specifies unknown property `{key}`");
                continue;
            }
            
            field.SetValue(this, Convert.ChangeType(value.StringValue(), field.PropertyType));
        }
    }

    public override string invalidMessage {
        get {
            if (id == null) return "Missing required property `id`";
            if (prettyName == null) return "Missing required property `prettyName`";
            if (bufferFrames == 0) return "Move must at least have 1 frame of buffer.";
            if (priority == -1) return "Priority must be set.";
            return null;
        }
    }
}
}
