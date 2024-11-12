using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Battle.FX {
public class CharacterFXManager : MonoBehaviour {
    [Title("References")]
    public CharacterFXLibrary fxLibrary;
    [Title("Sockets")]
    public UDictionary<CharacterFXSocketType, Transform> sockets = new();
    
    private Transform particleContainer; 
    private PlayerCharacter player;

    private void Start() {
        player = GetComponentInParent<PlayerCharacter>();

        particleContainer = new GameObject("Particles").transform;
        particleContainer.transform.parent = transform;
        particleContainer.transform.localPosition = Vector3.zero;
    }

    public void PlayGameObjectFX(GameObject prefab, CharacterFXSocketType type, Vector3 offset = default, Vector3 direction = default) {
        var socket = sockets[type];
        var go = Instantiate(prefab, socket);
        
        go.transform.localPosition = Vector3.zero + offset;
        go.transform.eulerAngles += direction;
        // print(go.GetComponent<MeshRenderer>());
        
    }

    public void NotifyHit(StandardAttackResult result) {
        var attack = result.attack;
        int level = attack.GetAttackLevel(result.to);
        bool isLargeHit = level >= 3;

        if (isLargeHit) {
            PlayGameObjectFX(fxLibrary.particleOnAnyHit, CharacterFXSocketType.DIRECTIONAL_SELF);
            SimpleCameraShakePlayer.inst.Play(fxLibrary.cameraShakeOnHitMedium);
        } else {
            
            SimpleCameraShakePlayer.inst.Play(fxLibrary.cameraShakeOnHitSmall);
        }
        
        switch (result.result) {
            case AttackResult.BLOCKED:
                PlayGameObjectFX(fxLibrary.particleOnBlock, CharacterFXSocketType.DIRECTIONAL_SELF);
                PlayGameObjectFX(fxLibrary.managedAttackBlock, CharacterFXSocketType.DIRECTIONAL_SELF);
                break;
            
            case AttackResult.HIT:
                if (isLargeHit) {
                    PlayGameObjectFX(fxLibrary.particleOnHitMedium, CharacterFXSocketType.DIRECTIONAL_SELF_TAIL);
                    PlayGameObjectFX(fxLibrary.particleOnLargerHitDirectional, 
                                     CharacterFXSocketType.DIRECTIONAL_SELF_TAIL, 
                                     new(-0, 0, 0),  
                                     new(0, -90, 0));
                } else {
                    PlayGameObjectFX(fxLibrary.particleOnHitSmall, CharacterFXSocketType.DIRECTIONAL_SELF);
                }
                break;
        }
        
    }
    
}

public enum CharacterFXSocketType {
    SELF,
    DIRECTIONAL_SELF,
    DIRECTIONAL_SELF_TAIL 
}
}
