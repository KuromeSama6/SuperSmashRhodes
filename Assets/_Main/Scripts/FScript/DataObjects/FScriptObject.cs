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
}
}
