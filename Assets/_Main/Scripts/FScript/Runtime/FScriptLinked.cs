using System;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Instruction;
using SuperSmashRhodes.FScript.Util;
using UnityEngine;

namespace SuperSmashRhodes.FScript {
/// <summary>
/// Represents the final version of FScript, with all references parsed and linked, at runtime.
/// </summary>
public class FScriptLinked {
    public string mainNamespace { get; private set; }
    public AddressRegistry addressRegistry { get; private set; }
    public Dictionary<string, SectionInstruction> sections { get; } = new();
    public Dictionary<string, int> labels { get; } = new();
    
    private Dictionary<string, FScriptObject> lib = new();
    
    public FScriptLinked(FScriptObject entry, List<FScriptLibrary> lib) {
        mainNamespace = entry.name;
        addressRegistry = new AddressRegistry();

        foreach (var library in lib) {
            foreach (var move in library.scripts.ToArray()) {
                if (move == null) continue;
                this.lib[move.name] = move;
            }
        }

        ParseScriptFile(entry.name, entry.rawScript);
    }

    private void ParseScriptFile(string @namespace, string content) {
        string[] lines = content.Split("\n");
        FInstruction lastInstruction = null;
        
        foreach (var line in lines) {
            var trimmed = line.Trim();
            if (trimmed.Length == 0) continue;
            if (trimmed.StartsWith(";")) continue;
            
            // directives
            if (trimmed.StartsWith("#")) {
                string[] args = trimmed.Split();
                if (args.Length == 0)
                    throw new FScriptException($"Empty directive");
                
                string directive = args[0].TrimStart('#');
                switch (directive.ToLower()) {
                    case "include":
                        if (args.Length < 2)
                            throw new FScriptException($"#include directive expected a script name");

                        string fileName = args[1];
                        if (!lib.ContainsKey(fileName)) 
                            throw new FScriptException($"Could not resolve #include script {fileName}");
                        FScriptObject script = lib[fileName];
                        
                        // Debug.Log($"linking script {fileName}");
                        ParseScriptFile(script.name, script.rawScript);
                        break;
                    
                    default:
                        throw new FScriptException($"Unknown directive {trimmed}");
                }
                
                continue;
            }
            
            // process
            int addr = addressRegistry.AllocateAddress();
            var fline = new FLine(trimmed);
            var instruction = FInstructionRegistry.InstantiateInstruction(fline, addr);
            addressRegistry.RegisterManaged(instruction);

            if (instruction is SectionInstruction section) {
                if (sections.ContainsKey(section.sectionName))
                    throw new FScriptException($"Duplicate section `{section.sectionName}`");

                var finalName = $"{@namespace}::{section.sectionName}";
                sections[finalName] = section;
                section.sectionName = finalName;

            } else if (instruction is LabelInstruction label) {
                if (labels.ContainsKey(label.labelName))
                    throw new FScriptException($"Duplicate label `{label.labelName}`");
                labels[$"{@namespace}::{label.labelName}"] = addr;
            }

            if (lastInstruction != null) {
                // Debug.Log($"addr {addr:X}, last {lastInstruction}");
                if (instruction is SectionInstruction) {
                    lastInstruction.nextAddress = 0;
                } else {
                    lastInstruction.nextAddress = addr;
                }
            }
            
            lastInstruction = instruction;
        }
    }
}

public static class ReservedSection {
    public static readonly string GLOBAL_INIT = "start";
}
}
