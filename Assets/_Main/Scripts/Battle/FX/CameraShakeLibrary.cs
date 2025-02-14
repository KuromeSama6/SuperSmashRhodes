using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Battle.FX {
[CreateAssetMenu(fileName = "New CameraShakeLib", menuName = "SSR/FX/CameraShakeLib", order = 0)]
public class CameraShakeLibrary : ScriptableObject {
    public UDictionary<string, SimpleCameraShakeData> shakes = new();
}
}
