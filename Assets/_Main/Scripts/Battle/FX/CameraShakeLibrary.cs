using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.Battle.FX {
[CreateAssetMenu(menuName = "Battle/FX/Camera Shake Lib", order = 0)]
public class CameraShakeLibrary : ScriptableObject {
    [Title("Simple Shakes")]
    public SimpleCameraShakeData smallHit;
    public SimpleCameraShakeData mediumHit, largeHit, largeCounterHit;
}

[Serializable]
public struct SimpleCameraShakeData {
    public AnimationCurve shakeCurve;
    public Vector3 velocity;
}
}
