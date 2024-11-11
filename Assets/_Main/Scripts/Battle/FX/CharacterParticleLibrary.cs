using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.Battle.FX {
[CreateAssetMenu(menuName = "Battle/FX/Character Particle Lib", order = 0)]
public class CharacterParticleLibrary : ScriptableObject {
    [Title("Particle Prefabs")]
    public GameObject anyHit;
    public GameObject normalBlock;
    public GameObject lightHit;
}
}
