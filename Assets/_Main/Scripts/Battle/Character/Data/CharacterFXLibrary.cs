using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.Battle.FX {
[CreateAssetMenu(menuName = "SSR/Battle/Character FX Lib", order = 0)]
public class CharacterFXLibrary : ScriptableObject {
    [Title("Camera")]
    public SimpleCameraShakeData cameraShakeOnHitSmall;
    public SimpleCameraShakeData cameraShakeOnHitMedium;
    public SimpleCameraShakeData cameraShakeOnHitLarge;
    
    [Title("Particles")]
    public GameObject particleOnAnyHit;
    public GameObject particleOnHitSmall;
    public GameObject particleOnHitMedium;
    public GameObject particleOnBlock;
    public GameObject particleOnLargerHitDirectional;
    public GameObject onWhiteForceReset;
    
    [Title("Managed")]
    public GameObject managedAttackBlock;
}
}
