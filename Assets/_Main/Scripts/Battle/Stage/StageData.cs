using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Validation;
using UnityEngine;

namespace SuperSmashRhodes.Battle.Stage {
[CreateAssetMenu(menuName = "SSR/Battle/Stage Data")]
public class StageData : ScriptableObject {
    [Title("References")]
    public string sceneId;
    
    [Title("Terrain")]
    public float midscreenWallDistanceMax = 12f;
    public float cornerWallX = 30f;
}
}
