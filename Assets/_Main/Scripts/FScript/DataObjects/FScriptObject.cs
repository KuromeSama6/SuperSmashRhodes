using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.FScript.Components;
using Unity.VisualScripting;
using UnityEngine;

namespace SuperSmashRhodes.FScript {
[System.Serializable]
public class FScriptObject : ScriptableObject {
    [HideInInspector]
    public string rawScript;
    public bool loaded { get; private set; }
    public bool ready { get; private set; }
    public FScriptDescriptor descriptor { get; private set; }
    public FScriptInputMethod inputMethod { get; private set; }
    public FScriptMainProcedure main { get; private set; }
    
    public Dictionary<string, FScriptSubroutine> subroutines { get; } = new();
    public Dictionary<string, FBlock> blocks { get; } = new();
    
    public void Load() {
        loaded = true;
        blocks.Clear();
        descriptor = null;
        
        string[] lines = rawScript.Split("\n");
        List<string> buf = null;
        
        foreach (var line in lines) {
            var trimmed = line.Trim();
            if (trimmed.Length == 0) continue;
            if (trimmed.StartsWith(";")) continue;
            
            bool tabbed = line.StartsWith("    ");
            
            if (!tabbed) {
                if (buf != null) {
                    // block end
                    var block = new FBlock(buf.ToArray());
                    blocks.Add(block.label, block);
                    buf.Clear();
                    
                } else {
                    buf = new List<string>();
                }
                
                buf.Add(trimmed);
                
            } else {
                if (buf == null) {
                    throw new FScriptException("FScript must start with a block");
                }
                buf.Add(trimmed);
            }

        }

        if (buf != null && buf.Count > 0) {
            var block = new FBlock(buf.ToArray());
            blocks[block.label] = block;
        }
        
        // process  blocks
        foreach (var block in blocks.Values) {
            if (block.label == "data") descriptor = new FScriptDescriptor(block);
            if (block.label == "input") inputMethod = new FScriptInputMethod(block);
            if (block.label == "main") main = new FScriptMainProcedure(block);
        }
        
        ready = true;
    }
}
}
