using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.FScript;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
/// <summary>
/// A MoveSet is a collections of moves that a character can perform. MoveSets hold multiple
/// FScript move objects. An Entity may have multiple MoveSets, which will be combined into a single
/// pool at runtime.
/// </summary>
[CreateAssetMenu(fileName = "MoveSet", menuName = "Battle/Move Set")]
public class MoveSet : ScriptableObject {
    [Title("FScript References")]
    public List<FScriptObject> moves = new();
}
}
