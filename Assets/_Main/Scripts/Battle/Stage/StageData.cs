using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.Battle.Stage {
[CreateAssetMenu(menuName = "SSR/Battle/Stage Data")]
public class StageData : ScriptableObject {
    public float midscreenWallDistanceMax = 12f;
    public float cornerWallX = 30f;
}
}
