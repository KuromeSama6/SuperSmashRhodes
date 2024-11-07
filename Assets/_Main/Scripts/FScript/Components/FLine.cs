using System.Linq;
using UnityEngine;

namespace SuperSmashRhodes.FScript.Components {
/// <summary>
/// Represents a line of FScript. The most basic building block of FScript.
/// Consists of a single instruction followed by arguments seperated by commas.
/// </summary>
public class FLine {
    public string instruction { get; private set; }
    public FImmediate[] args { get; private set; } = new FImmediate[0];
    public string raw { get; private set; }
    
    public FLine(string str) {
        raw = str;
        if (str.Length == 0)
            throw new FScriptException("Empty line");

        string[] words = str.Split(" ");
        instruction = words[0];
        if (words.Length == 1) return;
        
        string[] args = string.Join(' ', words[1..]).Split(",");
        this.args = (from c in args select new FImmediate(c.Trim())).ToArray();
    }

    public override string ToString() {
        return $"FLine({raw})";
    }
}
}
