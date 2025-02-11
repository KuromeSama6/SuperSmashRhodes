using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
[CreateAssetMenu( menuName = "SSR/Battle/Character Config", order = 0)]
public class CharacterConfiguration : ScriptableObject {
    public string prettyName;
    [SerializeField]
    private float defenseModifier = 1f;
    [SerializeField]
    private float baseGravity = 1.9f;
    public int guts;
    
    [TabGroup("playerConfig", "Grounded Movement", SdfIconType.ArrowsMove, TextColor = "green")]
    [SerializeField]
    private float walkSpeed, backwalkSpeed;

    [TabGroup("playerConfig", "Jumping", SdfIconType.ArrowUp, TextColor = "blue")]
    [SerializeField]
    private int prejump;
    [TabGroup("playerConfig", "Jumping", SdfIconType.ArrowUp, TextColor = "blue")]
    [SerializeField]
    private float jumpDuration, jumpVelocity, jumpGravity;

    [TabGroup("playerConfig", "Dash", SdfIconType.ArrowRight, TextColor = "yellow")]
    public bool mayDash = true;
    [TabGroup("playerConfig", "Dash", SdfIconType.ArrowRight, TextColor = "yellow")]
    public AnimationCurve dashAccelCurve;
    
    [TabGroup("playerConfig", "Dash", SdfIconType.ArrowRight, TextColor = "yellow")]
    [SerializeField]
    private int backdashInvuln;
    [TabGroup("playerConfig", "Dash", SdfIconType.ArrowRight, TextColor = "yellow")]
    [SerializeField]
    private float dashSpeed, airDashAvailableFrame, airDashDuration;
    [TabGroup("playerConfig", "Dash", SdfIconType.ArrowRight, TextColor = "yellow")]
    [SerializeField]
    private Vector2 backdashVelocity;
    
    public float defenseModifierFinal => 1f + defenseModifier;
    public float baseGravityFinal => 2.3f + baseGravity;
    public float walkSpeedFinal => 1 + walkSpeed;
    public float backwalkSpeedFinal => 0.8f + backwalkSpeed;
    public int prejumpFinal => 4 + prejump;
    public float jumpDurationFinal => 41 + jumpDuration;
    public float jumpVelocityFinal => 9 + jumpVelocity;
    public float jumpGravityFinal => 0f + jumpGravity;
    public int backdashInvulnFinal => 5 + backdashInvuln;
    public float dashSpeedFinal => 3 + dashSpeed;
    public float airDashAvailableFrameFinal => 0 + airDashAvailableFrame;
    public float airDashDurationFinal => 0 + airDashDuration;
    public Vector2 backdashVelocityFinal => new Vector2(-3, 3) + backdashVelocity;
}
}
