using System.Collections.Generic;
using UnityEngine;

namespace SuperSmashRhodes.FScript.Components {
/// <summary>
/// Represents a block of FScript, with a label and numerous lines.
/// </summary>
public class FBlock {
    public string label { get; private set; }
    public FLine[] lines { get; private set; } = new FLine[0];
    public bool isEmpty => lines.Length == 0;
    
    public FBlock(string[] lines) {
        if (lines.Length == 0)
            throw new FScriptException("Block cannot be empty.");
        
        string startLine = lines[0];
        if (!startLine.StartsWith("."))
            throw new FScriptException("Block must start with a comma(.)");
        if (!startLine.EndsWith(":"))
            throw new FScriptException("Block must end with a colon(:)");

        label = startLine.TrimStart('.').TrimEnd(':');
        
        // lines
        List<FLine> fLines = new();
        for (int i = 1; i < lines.Length; i++) {
            string line = lines[i];
            fLines.Add(new(line));
        }
        this.lines = fLines.ToArray();

    }
    
}

}
