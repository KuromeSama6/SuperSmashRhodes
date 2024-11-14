using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
[CreateAssetMenu( menuName = "SSR/Battle/Character Config", order = 0)]
public class CharacterConfiguration : ScriptableObject {
    public string prettyName;
    public float defenseModifier = 1f;
    public float baseGravity = 1.9f;
    
    [TabGroup("playerConfig", "Grounded Movement", SdfIconType.ArrowsMove, TextColor = "green")]
    public float walkSpeed, backwalkSpeed;

    [TabGroup("playerConfig", "Jumping", SdfIconType.ArrowUp, TextColor = "blue")]
    public int prejump;
    [TabGroup("playerConfig", "Jumping", SdfIconType.ArrowUp, TextColor = "blue")]
    public float jumpDuration, jumpVelocity, jumpGravity;

    [TabGroup("playerConfig", "Dash", SdfIconType.ArrowRight, TextColor = "yellow")]
    public bool mayDash = true;
    [TabGroup("playerConfig", "Dash", SdfIconType.ArrowRight, TextColor = "yellow")]
    public AnimationCurve dashAccelCurve;
    
    [TabGroup("playerConfig", "Dash", SdfIconType.ArrowRight, TextColor = "yellow")]
    public float dashSpeed, airDashAvailableFrame, airDashDuration, backdashDuration, backdashInvuln, backdashAirborne, backdashDistance;
}
}
