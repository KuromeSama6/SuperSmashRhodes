using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State {
[CreateAssetMenu(menuName = "Battle/Entity State Library")]
public class EntityStateLibrary : ScriptableObject {
    [ShowInInspector]
    public List<string> states = new();
}
}
