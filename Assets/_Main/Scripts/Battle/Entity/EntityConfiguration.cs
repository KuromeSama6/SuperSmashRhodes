using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
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
    public float prejump, jumpDuration, highJumpDuration, jumpHeight, highJumpHeight, jumpGravity, highJumpGravity;
    
    [ShowIf("entityType", EntityType.CHARACTER)]
    [TabGroup("playerConfig", "Dash", SdfIconType.ArrowRight, TextColor = "yellow")]
    public float dashSpeed, airDashAvailableFrame, airDashDuration, backdashDuration, backdashInvuln, backdashAirborne, backdashDistance;

}

public enum EntityType {
    CHARACTER,
    PROJECTILE,
    SUMMON
}
}
