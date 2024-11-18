using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.Battle.FX {
[CreateAssetMenu(menuName = "SSR/Battle/Character FX Lib", order = 0)]
public class CharacterFXLibrary : ScriptableObject {
    [Title("Camera")]
    public SimpleCameraShakeData cameraShakeOnHitSmall;
    public SimpleCameraShakeData cameraShakeOnHitMedium;
    public SimpleCameraShakeData cameraShakeOnHitLarge;
}
}
