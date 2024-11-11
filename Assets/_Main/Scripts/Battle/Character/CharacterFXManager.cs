using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.Enums;
using UnityEngine;

namespace SuperSmashRhodes.Battle.FX {
public class CharacterFXManager : MonoBehaviour {
    [Title("References")]
    public CharacterParticleLibrary particleLibrary;
    
    private Transform particleContainer;
    private PlayerCharacter player;

    private void Start() {
        player = GetComponentInParent<PlayerCharacter>();

        particleContainer = new GameObject("Particles").transform;
        particleContainer.transform.parent = transform;
        particleContainer.transform.localPosition = Vector3.zero;
    }

    public void PlayParticleVFX(GameObject prefab, ParticleTargetType type) {
        var go = Instantiate(prefab);
        var height = 1f;
        
        switch (type) {
            case ParticleTargetType.INTERACTION:
                float direction = player.side == EntitySide.LEFT ? 1 : -1;
                go.transform.position = player.transform.position + new Vector3(player.pushboxManager.pushboxSize * direction, height, 0);
                break;
            
            default:
                go.transform.position = player.transform.position + new Vector3(0, height, 0);
                break;
        }
        
        // print(go.GetComponent<MeshRenderer>());
        
    }

    public void NotifyHit() {
        PlayParticleVFX(particleLibrary.anyHit, ParticleTargetType.INTERACTION);
        PlayParticleVFX(particleLibrary.lightHit, ParticleTargetType.INTERACTION);
    }
    
    public void NotifyBlock() {
        PlayParticleVFX(particleLibrary.anyHit, ParticleTargetType.INTERACTION);
        PlayParticleVFX(particleLibrary.normalBlock, ParticleTargetType.INTERACTION);
    }
    
}

public enum ParticleTargetType {
    SELF,
    INTERACTION
}
}
