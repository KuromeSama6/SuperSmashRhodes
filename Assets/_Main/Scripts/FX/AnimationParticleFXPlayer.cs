using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.FX {
public class AnimationParticleFXPlayer : MonoBehaviour {
    [Title("Configuration")]
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    
    private Animation anim;

    private void Start() {
        anim = GetComponent<Animation>();
        transform.localPosition += positionOffset;
        transform.localEulerAngles += rotationOffset;
    }

    private void Update() {
        if (!anim.isPlaying) {
            Destroy(gameObject);
        }
    }
}
}
