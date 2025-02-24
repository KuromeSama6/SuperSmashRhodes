using System;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.FX;
using SuperSmashRhodes.Runtime.State;
using SuperSmashRhodes.Util;

using UnityEngine;

namespace SuperSmashRhodes.Battle.FX {
public class CharacterFXManager : MonoBehaviour {
    private static readonly int SHOW_EMBLEM_FX = Animator.StringToHash("Show");
    [Title("Sockets")]
    public UDictionary<CharacterFXSocketType, Transform> sockets = new();
    // [BoxGroup("Direct Managed")]
    // public MMF_Player staticOnGroundedTechFlashPlayer;
    
    [BoxGroup("Emblems")]
    public GameObject superEmblem, driveReleaseEmblem;
    
    public bool playFlash => player && player.activeState != null && (player.activeState.invincibility & AttackType.FULL).HasFlag(AttackType.FULL);
    
    private Transform particleContainer; 
    private PlayerCharacter player;

    private void Start() {
        player = GetComponentInParent<PlayerCharacter>();

        particleContainer = new GameObject("Particles").transform;
        particleContainer.transform.parent = transform;
        particleContainer.transform.localPosition = Vector3.zero;

        superEmblem.GetComponentInChildren<SpriteRenderer>().sprite = player.descriptor.emblem;
        driveReleaseEmblem.GetComponentInChildren<SpriteRenderer>().sprite = player.descriptor.emblem;
    }

    private void Update() {

    }

    public void PlayGameObjectFX(string key, CharacterFXSocketType type, Vector3 offset = default, Vector3 direction = default, Vector3? scale = null) {
        AssetManager.Get<GameObject>(key, (prefab) => {
            PlayGameObjectFX(prefab, type, offset, direction, scale);
        });
    }

    public void PlayGameObjectFXAtSocket(string key, String socket, Vector3 offset = default, Vector3 direction = default, Vector3? scale = null) {
        AssetManager.Get<GameObject>(key, (prefab) => {
            var target = player.socketsContainer.Find(socket);
            if (target == null) {
                Debug.LogError($"Socket {socket} not found");
                return;
            }
            
            PlayGameObjectFX(prefab, CharacterFXSocketType.WORLD_UNBOUND, target.position + offset, direction, scale);
        });
    }
    
    public void PlayFlipbookFX(string key, CharacterFXSocketType type, Vector3 offset = default, Vector3 direction = default, Vector3? scale = null, string sortingLayer = "BehindCharacter") {
        AssetManager.Get<FlipbookData>(key, data => {
            var go = new GameObject($"Flipbook_{key}");
            if (scale != null) {
                go.transform.localScale = scale.Value;
            }
            
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = sortingLayer;
            
            var player = go.AddComponent<FlipbookPlayer>();
            player.flipbookData = data;
            player.destroyOnFinish = true;
            player.Play();
            
            PlayGameObjectFX(go, type, offset, direction, scale, false);
        });
    }
    
    public void PlayGameObjectFX(GameObject prefab, CharacterFXSocketType type, Vector3 offset = default, Vector3 direction = default, Vector3? scale = null, bool isPrefab = true) {
        var socket = type == CharacterFXSocketType.WORLD || type == CharacterFXSocketType.WORLD_UNBOUND || type == CharacterFXSocketType.WORLD_UNBOUND_RELATIVE ? null : sockets[type];
        var go = isPrefab ? Instantiate(prefab, socket) : prefab;
        
        if (type == CharacterFXSocketType.WORLD_UNBOUND) {
            go.transform.position = offset;
            go.transform.position = GameManager.inst.ClampPositionToStage(go.transform.position);
            go.transform.localEulerAngles = direction;

        }

        if (type == CharacterFXSocketType.WORLD_UNBOUND_RELATIVE) {
            go.transform.position = player.transform.position + offset;
            go.transform.position = GameManager.inst.ClampPositionToStage(go.transform.position);
            go.transform.localEulerAngles = direction;
        }
        
        if (type == CharacterFXSocketType.WORLD) {
            go.transform.position = player.transform.position + new Vector3(0, 1, 0) + PhysicsUtil.NormalizeSide(offset, player.side);
            go.transform.position = GameManager.inst.ClampPositionToStage(go.transform.position);
            
        } else {
            go.transform.localPosition = Vector3.zero + offset;
        }
        
        // if (player.activeState is State_CmnHitStunAir) offset += new Vector3(0, -.5f, 0);
        go.transform.localEulerAngles += direction;
        // print(go.GetComponent<MeshRenderer>());
        if (scale != null) {
            go.transform.localScale = scale.Value;
        }

    }

    public void NotifyHit(AttackData data) {
        var attack = data.attack;
        int level = attack.GetAttackLevel(data.to);
        bool isLargeHit = level >= 2;

        if (isLargeHit) {
            PlayGameObjectFX("cmn/battle/fx/prefab/common/attack_land/0", CharacterFXSocketType.DIRECTIONAL_SELF);
        }

        switch (attack.GetAttackLevel(data.to)) {
            case 1:
                SimpleCameraShakePlayer.inst.PlayCommon("hit_small");
                break;
            case 2:
            case 3:
                SimpleCameraShakePlayer.inst.PlayCommon("hit_medium");
                break;
            case 4:
                SimpleCameraShakePlayer.inst.PlayCommon("hit_large");
                break;
        }
        
        switch (data.result) {
            case AttackResult.BLOCKED:
                PlayGameObjectFX("cmn/battle/fx/prefab/common/block/0", CharacterFXSocketType.DIRECTIONAL_SELF);
                PlayGameObjectFX("cmn/battle/fx/prefab/common/block/1", CharacterFXSocketType.DIRECTIONAL_SELF);
                break;
            
            case AttackResult.HIT:
                if (isLargeHit) {
                    PlayGameObjectFX("cmn/battle/fx/prefab/common/hit/medium", CharacterFXSocketType.DIRECTIONAL_SELF_TAIL);
                    PlayGameObjectFX("cmn/battle/fx/prefab/common/hit/directional", 
                                     CharacterFXSocketType.DIRECTIONAL_SELF_TAIL, 
                                     new(-0, 0, 0),  
                                     new(0, -90, 0));
                } else {
                    PlayGameObjectFX("cmn/battle/fx/prefab/common/hit/light", CharacterFXSocketType.SELF);
                }
                break;
        }
        
    }

    public void PlayEmblemFX(float duration, bool isSuper) {
        var go = isSuper ? superEmblem : driveReleaseEmblem;
        var animator = go.GetComponentInChildren<Animator>();
        animator.SetBool(SHOW_EMBLEM_FX, true);
        
        this.CallLaterCoroutine(duration, () => {
            animator.SetBool(SHOW_EMBLEM_FX, false);
        });
    }
    
}

public enum CharacterFXSocketType {
    SELF,
    DIRECTIONAL_SELF,
    DIRECTIONAL_SELF_TAIL,
    WORLD,
    WORLD_UNBOUND,
    WORLD_UNBOUND_RELATIVE
}
}
