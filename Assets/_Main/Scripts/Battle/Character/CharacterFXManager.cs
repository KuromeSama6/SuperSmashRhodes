using System;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Runtime.State;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Battle.FX {
public class CharacterFXManager : MonoBehaviour {
    [Title("References")]
    public CharacterFXLibrary fxLibrary;
    [Title("Sockets")]
    public UDictionary<CharacterFXSocketType, Transform> sockets = new();
    [BoxGroup("Direct Managed")]
    public MMF_Player staticOnGroundedTechFlashPlayer;
    
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

        // if (player.activeState is State_CmnHitStunAir) offset += new Vector3(0, -.5f, 0);
        go.transform.localPosition = Vector3.zero + offset;
        go.transform.eulerAngles += direction;
        // print(go.GetComponent<MeshRenderer>());
        
    }

    public void NotifyHit(AttackData data) {
        var attack = data.attack;
        int level = attack.GetAttackLevel(data.to);
        bool isLargeHit = level >= 2;

        if (isLargeHit) {
            PlayGameObjectFX(fxLibrary.particleOnAnyHit, CharacterFXSocketType.DIRECTIONAL_SELF);
        }

        switch (attack.GetAttackLevel(data.to)) {
            case 1:
                SimpleCameraShakePlayer.inst.Play(fxLibrary.cameraShakeOnHitSmall);
                break;
            case 2:
            case 3:
                SimpleCameraShakePlayer.inst.Play(fxLibrary.cameraShakeOnHitMedium);
                break;
            case 4:
                SimpleCameraShakePlayer.inst.Play(fxLibrary.cameraShakeOnHitLarge);
                break;
        }
        
        switch (data.result) {
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
