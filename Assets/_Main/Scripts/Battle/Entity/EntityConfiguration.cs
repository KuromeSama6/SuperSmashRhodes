using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes._Main.Scripts.Battle.Animation;
using UnityEngine;
using UnityEngine.Serialization;

namespace SuperSmashRhodes.Battle {
[Obsolete]
[CreateAssetMenu(fileName = "EntityConfiguration", menuName = "Battle/Entity Configuration")]
public class EntityConfiguration : ScriptableObject {
    [BoxGroup("Basic Configuration")]
    public string id, prettyName;
    [BoxGroup("Basic Configuration")]
    public EntityType entityType;

    [BoxGroup("Basic Stats")]
    public float health, defenseModifier;

    [ShowIf("entityType", EntityType.CHARACTER)]
    [TabGroup("playerConfig", "Grounded Movement", SdfIconType.ArrowsMove, TextColor = "green")]
    public float walkSpeed, backwalkSpeed;

    [ShowIf("entityType", EntityType.CHARACTER)]
    [TabGroup("playerConfig", "Jumping", SdfIconType.ArrowUp, TextColor = "blue")]
    public float prejump, jumpDuration, jumpVelocity, jumpGravity;

    [ShowIf("entityType", EntityType.CHARACTER)]
    [TabGroup("playerConfig", "Dash", SdfIconType.ArrowRight, TextColor = "yellow")]
    public AnimationCurve dashAccelCurve;
    
    [ShowIf("entityType", EntityType.CHARACTER)]
    [TabGroup("playerConfig", "Dash", SdfIconType.ArrowRight, TextColor = "yellow")]
    public float dashSpeed, airDashAvailableFrame, airDashDuration, backdashDuration, backdashInvuln, backdashAirborne, backdashDistance;

    [FormerlySerializedAs("clipReferences")]
    [BoxGroup("Animation")]
    public List<AnimationClipReference> animationClipReferences = new();

}

public enum EntityType {
    CHARACTER,
    PROJECTILE,
    SUMMON
}
}
