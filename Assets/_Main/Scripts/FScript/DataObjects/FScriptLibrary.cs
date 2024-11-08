using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.FScript;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
[CreateAssetMenu(fileName = "FScript Library", menuName = "FScript/FScript Library")]
public class FScriptLibrary : ScriptableObject {
    [Title("FScript References")]
    public List<FScriptObject> scripts = new();

    public FScriptObject GetScript(string name) {
        return scripts.Find(s => s.name == name);
    }
}
}
