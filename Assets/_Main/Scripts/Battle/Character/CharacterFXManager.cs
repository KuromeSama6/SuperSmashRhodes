using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.Game;
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

    public void PlayGameObjectFX(string key, CharacterFXSocketType type, Vector3 offset = default, Vector3 direction = default) {
        AssetManager.Get<GameObject>(key, (prefab) => {
            PlayGameObjectFX(prefab, type, offset, direction);
        });
    }
    
    public void PlayGameObjectFX(GameObject prefab, CharacterFXSocketType type, Vector3 offset = default, Vector3 direction = default) {
        var socket = type == CharacterFXSocketType.WORLD ? null : sockets[type];
        var go = Instantiate(prefab, socket);

        if (type == CharacterFXSocketType.WORLD) {
            go.transform.position = player.transform.position + new Vector3(0, 1, 0) + PhysicsUtil.NormalizeSide(offset, player.side);
            go.transform.position = GameManager.inst.ClampPositionToStage(go.transform.position);
        } else {
            go.transform.localPosition = Vector3.zero + offset;
        }
        
        // if (player.activeState is State_CmnHitStunAir) offset += new Vector3(0, -.5f, 0);
        go.transform.localEulerAngles += direction;
        // print(go.GetComponent<MeshRenderer>());
        
    }

    public void NotifyHit(AttackData data) {
        var attack = data.attack;
        int level = attack.GetAttackLevel(data.to);
        bool isLargeHit = level >= 2;

        if (isLargeHit) {
            PlayGameObjectFX("cmn/batte/fx/prefab/common/attack_land/0", CharacterFXSocketType.DIRECTIONAL_SELF);
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
                PlayGameObjectFX("cmn/batte/fx/prefab/common/block/0", CharacterFXSocketType.DIRECTIONAL_SELF);
                PlayGameObjectFX("cmn/batte/fx/prefab/common/block/1", CharacterFXSocketType.DIRECTIONAL_SELF);
                break;
            
            case AttackResult.HIT:
                if (isLargeHit) {
                    PlayGameObjectFX("cmn/batte/fx/prefab/common/hit/medium", CharacterFXSocketType.DIRECTIONAL_SELF_TAIL);
                    PlayGameObjectFX("cmn/batte/fx/prefab/common/hit/directional", 
                                     CharacterFXSocketType.DIRECTIONAL_SELF_TAIL, 
                                     new(-0, 0, 0),  
                                     new(0, -90, 0));
                } else {
                    PlayGameObjectFX("cmn/batte/fx/prefab/common/hit/light", CharacterFXSocketType.DIRECTIONAL_SELF);
                }
                break;
        }
        
    }
    
}

public enum CharacterFXSocketType {
    SELF,
    DIRECTIONAL_SELF,
    DIRECTIONAL_SELF_TAIL,
    WORLD
}
}
