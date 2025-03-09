using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

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
    [TabGroup("playerConfig", "Jumping", SdfIconType.ArrowUp, TextColor = "blue")]
    public bool mayAirJump = true;

    [TabGroup("playerConfig", "Dash", SdfIconType.ArrowRight, TextColor = "yellow")]
    public bool mayDash = true, mayAirDash = true;
    [TabGroup("playerConfig", "Dash", SdfIconType.ArrowRight, TextColor = "yellow")]
    public AnimationCurve dashAccelCurve;
    
    [TabGroup("playerConfig", "Dash", SdfIconType.ArrowRight, TextColor = "yellow")]
    [SerializeField]
    private int backdashInvuln, airOptions, airDashCancellableFrame, airDashAvailableFrame, airDashDuration, airBackdashDuration;
    [TabGroup("playerConfig", "Dash", SdfIconType.ArrowRight, TextColor = "yellow")]
    [SerializeField]
    private float dashSpeed, airdashSpeed, airBackdashSpeed;
    [TabGroup("playerConfig", "Dash", SdfIconType.ArrowRight, TextColor = "yellow")]
    [SerializeField]
    private Vector2 backdashVelocity;
    
    public float defenseModifierFinal => 1f + defenseModifier;
    public float baseGravityFinal => 2.5f + baseGravity;
    public float walkSpeedFinal => 1 + walkSpeed;
    public float backwalkSpeedFinal => 0.8f + backwalkSpeed;
    public int prejumpFinal => 4 + prejump;
    public float jumpDurationFinal => 41 + jumpDuration;
    public float jumpVelocityFinal => 10 + jumpVelocity;
    public float jumpGravityFinal => 0f + jumpGravity;
    public int backdashInvulnFinal => 5 + backdashInvuln;
    public float dashSpeedFinal => 3 + dashSpeed;
    public int airDashAvailableFrameFinal => 7 + airDashAvailableFrame;
    public int airDashDurationFinal => 24 + airDashDuration;
    public int airDashCancellableFrameFinal => 9 + airDashCancellableFrame;
    public float airDashSpeedFinal => 7 + airdashSpeed;
    public float airBackdashSpeedFinal => 5.5f + airBackdashSpeed;
    public int airBackdashDurationFinal => 14 + airBackdashDuration;
    public int airOptionsFinal => 1 + airOptions;
    public Vector2 backdashVelocityFinal => new Vector2(-4.5f, 4) + backdashVelocity;
}
}
