using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.Stage;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Runtime.State;
using SuperSmashRhodes.UI.Battle;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace SuperSmashRhodes.Battle.Game {
public class GameManager : SingletonBehaviour<GameManager> {
    [Title("References")]
    [Title("Camera")]
    public CinemachineCamera mainCamera;
    public CinemachineTargetGroup targetGroup;

    [Title("Stage")]
    public StageData stageData;
    public GameObject ground;
    public GameObject leftWall, rightWall;
    public Transform environmentContainer;
    
    [Title("Debug")]
    public GameObject p1Prefab;
    public GameObject p2Prefab;

    public CharacterStateFlag globalStateFlags {
        get {
            var ret = CharacterStateFlag.NONE;
            foreach (var player in players.Values) {
                if (player.activeState == null) continue;
                ret |= player.activeState.globalFlags;
            }
            
            if (TimeManager.inst.globalFreezeFrames > 0) ret |= CharacterStateFlag.GLOBAL_PAUSE_TIMER | CharacterStateFlag.PAUSE_GAUGE;
            return ret;
        }
    }
    
    private PlayerInputManager inputManager;
    private Dictionary<int, PlayerCharacter> players = new();
    private bool pushboxCorrectionLock = false;
    private readonly Dictionary<int, Entity> entityTable = new();
    private int entityIdCounter;
    private readonly List<Renderer> environmentRenderers = new();
    
    private IEnumerator Start() {
        environmentRenderers.AddRange(environmentContainer.gameObject.GetComponentsInChildren<Renderer>());
        inputManager = GetComponent<PlayerInputManager>();
        
        targetGroup.Targets.Clear();
        
        CreatePlayer(0, p1Prefab);
        CreatePlayer(1, p2Prefab);

        yield return new WaitForFixedUpdate();
        foreach (var player in players.Values) {
            player.OnRoundInit();
        }
        foreach (var player in players.Values) {
            player.BeginLogic();
        }
        
        AssetManager.inst.PreloadAll("cmn/battle/sfx/**");
        AssetManager.inst.PreloadAll("cmn/battle/fx/**");
    }

    private void CreatePlayer(int index, GameObject prefab) {
        var input = Instantiate(prefab);
        var player = input.GetComponent<PlayerCharacter>();
        AssetManager.inst.PreloadAll($"chr/{player.config.id}/**");
        player.Init(index);
        player.name = "Player" + index;
        
        players[index] = player;
        targetGroup.AddMember(player.transform, 1, 0.5f);
        
        
    }
    
    public PlayerCharacter GetOpponent(PlayerCharacter player) {
        return players[player.playerIndex == 0 ? 1 : 0];
    }

    public PlayerCharacter GetPlayer(int index) {
        return players[index];
    }

    public Vector3 ClampPositionToStage(Vector3 position) {
        var x = Mathf.Clamp(position.x, leftWall.transform.position.x, rightWall.transform.position.x);
        return new Vector3(x, position.y, position.z);
    }

    private void Update() {
        foreach (var target in targetGroup.Targets) {
            var player = GetPlayer(targetGroup.Targets.IndexOf(target));
            target.Object = player.stateFlags.HasFlag(CharacterStateFlag.CAMERA_FOLLOW_BONE) ? player.cameraFollowSocket : player.transform;
            target.Weight = player.cameraGroupWeight;
        }

        {
            var hideTerrain = BackgroundUIManager.inst && BackgroundUIManager.inst.fullyDimmed;
            foreach (var renderer in environmentRenderers) {
                renderer.enabled = !hideTerrain;
            }   
        }
    }

    private void FixedUpdate() {
        pushboxCorrectionLock = false;
    }

    public void AttemptPushboxCorrection(PlayerCharacter top, PlayerCharacter bottom) {
        if (pushboxCorrectionLock) return;
        pushboxCorrectionLock = true;

        float direction;
        PlayerCharacter target;

        if (top.atWall) {
            target = bottom;
            direction = bottom.side == EntitySide.RIGHT ? 1 : -1;
            
        } else if (bottom.atWall) {
            target = top;
            direction = top.side == EntitySide.RIGHT ? 1 : -1;

        } else if (bottom.wallDistance < bottom.pushboxManager.correctionBox.size.x) {
            target = bottom;
            direction = top.side == EntitySide.RIGHT ? 1 : -1; 
            
        } else {
            target = top;
            if (top.side == EntitySide.LEFT) direction = top.transform.position.x > bottom.transform.position.x ? 1 : -1;
            else direction = top.transform.position.x < bottom.transform.position.x ? -1 : 1;
        }

        var other = target.opponent;

        var pos = other.transform.position;
        pos.y = target.transform.position.y;

        var offset = (other.pushboxManager.correctionBox.size.x + .05f);
        target.transform.position = pos + new Vector3(direction * offset, 0, 0);
        target.rb.linearVelocityX = 0;
        other.rb.linearVelocityX = 0;
        // Debug.Log($"target {target.playerIndex} target atwall {target.atWall} other {other.playerIndex} bottom {bottom.playerIndex} top {top.playerIndex}");
        target.SetZPriority();
        target.pushboxCorrectionGraceAmount = -direction * offset;
        target.UpdateRotation();
    }

    public void HandleWallCollision(Wall wall, PlayerCharacter player) {
        // wall bounce
        if (player.activeState is State_CmnHitStunAir && player.frameData.shouldWallBounce) {
            var force = player.frameData.ConsumeWallBounce();
            player.rb.linearVelocity = Vector2.zero;
            player.ApplyForwardVelocity(force.bounceForce);
            
            AssetManager.Get<GameObject>("cmn/battle/fx/prefab/common/wall_bounce", res => {
                var fx = Instantiate(res);
                fx.transform.position = wall.transform.position - new Vector3(wall.GetComponent<BoxCollider2D>().size.x * (wall.side == EntitySide.RIGHT ? 1f : -1f), 0, 0);
            });
            
            player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/wall_bounce_smoke", CharacterFXSocketType.WORLD_UNBOUND, player.transform.position, new Vector3(0, 0, wall.side == EntitySide.RIGHT ? 0 : 180));
            player.audioManager.PlaySound("cmn/battle/sfx/wall_bounce");
            player.airHitstunRotation = -90f;

            SimpleCameraShakePlayer.inst.PlayCommon("wallbounce");
            TimeManager.inst.Schedule(4, 10);
        }
    }
    
    public int RegisterEntity(Entity entity) {
        var id = entityIdCounter++;
        entityTable[id] = entity;
        return id;
    }
}


}
