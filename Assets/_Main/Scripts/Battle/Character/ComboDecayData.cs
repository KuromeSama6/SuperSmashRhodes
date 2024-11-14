using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
[CreateAssetMenu(menuName = "SSR/Battle/Combo Decay Data", order = 0)]
public class ComboDecayData : ScriptableObject {
    [Title("Curves")]
    [PropertyTooltip("Extra combo damage proration. [1.0f, 0.0f]")]
    public AnimationCurve extraProrationCurve;
    [PropertyTooltip("Opponent's gravity multiplier. [1.0f, inf)")]
    public AnimationCurve opponentGravityCurve;
    [PropertyTooltip("Horizontal blowback of future attacks. [1.0f, inf)")]
    public AnimationCurve opponentBlowbackCurve;
    [PropertyTooltip("Vertical launch of future attacks. [1.0f, 0.0f]")]
    public AnimationCurve opponentLaunchCurve;
}
}
