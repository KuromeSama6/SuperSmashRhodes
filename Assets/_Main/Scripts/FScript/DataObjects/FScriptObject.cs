using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Util;
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
    public AddressRegistry addressRegistry { get; private set; }
    
    public void Load() {
        addressRegistry = new();
        loaded = true;
        subroutines.Clear();
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
            else if (block.label == "input") inputMethod = new FScriptInputMethod(block);
            else if (block.label == "main") main = new FScriptMainProcedure(block, addressRegistry);
            else {
                // subroutines
                if (!block.label.StartsWith("sub_"))
                    throw new FScriptException($"Illegal subroutine name {block.label}: Must begin with `_sub`");
                if (subroutines.ContainsKey(block.label))
                    throw new FScriptException($"Duplicate subroutine name {block.label}");
                
                subroutines[block.label] = new FScriptSubroutine(block, addressRegistry);
            }
        }
        
        ready = true;
    }
    
    public FScriptObject DetachedCopy() {
        var copy = CreateInstance<FScriptObject>();
        copy.rawScript = rawScript;
        copy.Load();
        return copy;
    }
}
}
