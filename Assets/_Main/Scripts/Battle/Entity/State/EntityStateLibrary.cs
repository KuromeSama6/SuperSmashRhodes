using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State {
[CreateAssetMenu(menuName = "SSR/Battle/Entity State Library")]
public class EntityStateLibrary : ScriptableObject {
    public string prefix;
    public bool useTokenNameAsPrefix;
    public List<string> states = new();
}
}
