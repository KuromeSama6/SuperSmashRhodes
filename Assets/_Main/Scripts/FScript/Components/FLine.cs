using System.Linq;
using SuperSmashRhodes.FScript.Util;
using UnityEngine;

namespace SuperSmashRhodes.FScript.Components {
/// <summary>
/// A FLine represents a single line of FScript code, and is the most basic building blocks of FScript.
/// </summary>
public class FLine {
    public string instruction { get; private set; }
    public FImmediate[] args { get; private set; } = new FImmediate[0];
    public string raw { get; private set; }
    public int address { get; private set; }
    
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
