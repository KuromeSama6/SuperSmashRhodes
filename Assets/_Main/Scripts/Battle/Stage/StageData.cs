using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.Battle.Stage {
[CreateAssetMenu(menuName = "SSR/Battle/Stage Data")]
public class StageData : ScriptableObject {
    [Title("Walls")]
    public float leftWallPosition;
    public float rightWallPosition;
}
}
