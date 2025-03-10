using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.State;
using UnityEngine;
using UnityEngine.Serialization;

namespace SuperSmashRhodes.Battle {
[CreateAssetMenu(fileName = "EntityConfiguration", menuName = "SSR/Battle/Entity Configuration")]
public class EntityConfiguration : ScriptableObject {
    [BoxGroup("Basic Configuration")]
    public string id, tokenName;
    [BoxGroup("Basic Configuration")]
    [HideInInspector]
    [Obsolete]
    public EntityType entityType;

    [BoxGroup("State")]
    public List<EntityStateLibrary> stateLibraries = new();
}

public enum EntityType {
    CHARACTER,
    TOKEN,
    PROJECTILE,
    SUMMON,
    SUBATTACK,
    COSMETIC
}
}