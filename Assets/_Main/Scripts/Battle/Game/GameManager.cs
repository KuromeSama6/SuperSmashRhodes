using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Battle.Stage;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Network.Room;
using SuperSmashRhodes.Room;
using SuperSmashRhodes.Runtime.State;
using SuperSmashRhodes.Scripts.Audio;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.Util;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace SuperSmashRhodes.Battle.Game {
public class GameManager : SingletonBehaviour<GameManager>, IManualUpdate, IAutoSerialize {
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
    public RoomConfiguration debugRoomConfig;
    public StageData debugStageData;
    public StageBGMData debugBgmData;
    public CharacterDescriptor debugCharacterP1, debugCharacterP2;

    public CharacterStateFlag globalStateFlags {
        get {
            var ret = CharacterStateFlag.NONE;
            foreach (var player in players.Values) {
                if (player.activeState == null) continue;
                ret |= player.activeState.globalFlags;
            }
            
            if (TimeManager.inst.globalFreezeFrames > 0) ret |= CharacterStateFlag.GLOBAL_PAUSE_TIMER | CharacterStateFlag.PAUSE_GAUGE;
            
            return ret | extraGlobalStateFlags;
        }
    }
    
    public Dictionary<int, PlayerCharacter> players { get; } = new();
    private bool pushboxCorrectionLock = false;
    public bool inGame { get; set; } = false;
    
    private readonly Dictionary<int, EntityReference> entityTable = new();
    private readonly Dictionary<Entity, int> entityIdReassignTable = new();
    
    private int entityIdCounter;
    private readonly List<Renderer> environmentRenderers = new();
    public CinemachineGroupFraming cameraFraming { get; private set; }
    
    public CharacterStateFlag extraGlobalStateFlags { get; set; }
    
    private void Start() {
        environmentRenderers.AddRange(environmentContainer.gameObject.GetComponentsInChildren<Renderer>());
        cameraFraming = mainCamera.GetComponent<CinemachineGroupFraming>();
        targetGroup.Targets = new();
    }

    public void PreloadResources() {
        AssetManager.inst.PreloadAll("cmn/battle/sfx/**");
        AssetManager.inst.PreloadAll("cmn/battle/fx/**");
        AssetManager.inst.PreloadAll("cmn/announcer/battle/**");
        
        foreach (var player in RoomManager.inst.current.players.Values) {
            AssetManager.inst.PreloadAll($"chr/{player.selectedCharacter.id}/**");
        }
    }
    
    public void RoundInit() {
        StartCoroutine(InitRoundCoroutine());
    }

    public void ResetRound() {
        foreach (var player in players.Values) {
            Destroy(player.gameObject);
        }

        foreach (var entity in entityTable.Values) {
            Destroy(entity.entity.gameObject);
        }
        
        players.Clear();
        entityTable.Clear();
        entityIdReassignTable.Clear();
        
        targetGroup.Targets.Clear();
    }
    
    private IEnumerator InitRoundCoroutine() {
        var room = RoomManager.inst.current;
        
        CreatePlayer(0, room.players[0].selectedCharacter.gameObject);
        CreatePlayer(1, room.players[1].selectedCharacter.gameObject);
        
        yield return new WaitForFixedUpdate();
        
        foreach (var player in players.Values) {
            player.BeginLogic();
        }
    }
    
    public PlayerCharacter CreatePlayer(int index, GameObject prefab) {
        var input = Instantiate(prefab);
        var player = input.GetComponent<PlayerCharacter>();
        player.Init(index);
        player.name = "Player" + index;
        
        players[index] = player;
        targetGroup.AddMember(player.transform, 1, 0.5f);
        
        return player;
    }
    
    public PlayerCharacter GetOpponent(PlayerCharacter player) {
        return players.GetValueOrDefault(player.playerIndex == 0 ? 1 : 0);
    }

    public PlayerCharacter GetPlayer(int index) {
        return players.GetValueOrDefault(index);
    }

    public Vector3 ClampPositionToStage(Vector3 position) {
        var x = Mathf.Clamp(position.x, leftWall.transform.position.x, rightWall.transform.position.x);
        return new Vector3(x, position.y, position.z);
    }

    public void ManualUpdate() {
        if (RoomManager.inst.current == null && UnityEngine.Input.GetKeyDown(KeyCode.F6) && debugRoomConfig) {
            print("begin debug match");
            BeginDebugMatch();
        }
        
        if (!inGame) return;
        
        foreach (var target in targetGroup.Targets) {
            var player = GetPlayer(targetGroup.Targets.IndexOf(target));
            if (player.stateFlags.HasFlag(CharacterStateFlag.CAMERA_FOLLOWS_BONE)) {
                var socket = player.activeState.stateData.cameraData.focusBone == null ? player.cameraFollowSocket : player.GetHopBoneFollower(player.activeState.stateData.cameraData.focusBone);
                target.Object = socket.transform;

            } else {
                target.Object = player.transform;   
            }
            
            target.Weight = player.cameraGroupWeight;
        }

        {
            var fov = 110f;
            foreach (var player in players.Values) {
                if (player.activeState == null) continue;
                fov += player.activeState.stateData.cameraData.cameraFovModifier;
            }

            cameraFraming.FovRange = new(fov, fov);
        }
        
        {
            var hideTerrain = BackgroundUIManager.inst && BackgroundUIManager.inst.fullyDimmed;
            foreach (var renderer in environmentRenderers) {
                renderer.enabled = !hideTerrain;
            }   
        }
        
        PruneEntities();
    }

    public void ManualFixedUpdate() {
        pushboxCorrectionLock = false;
        if (RoomManager.inst.current != null) {
            RoomManager.inst.current.Tick();
        }
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
        if (!player) return;
        
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
    
    public EntityReference RegisterEntity(Entity entity) {
        if (entityIdReassignTable.ContainsKey(entity)) {
            var reference = entityTable[entityIdReassignTable[entity]];
            entityIdReassignTable.Remove(entity);
            return reference;
        }
        
        var id = entityIdCounter++;
        var ret = new EntityReference(entity, id);
        entityTable[id] = ret;
        return ret;
    }
    
    public void UnregisterEntity(Entity entity) {
        entityTable[entity.entityId].alive = false;
        Destroy(entity.gameObject);
    }

    public Entity ResolveEntity(EntityHandle handle) {
        // Debug.Log($"resolve handle, {handle}");
        if (!entityTable.ContainsKey(handle.entityId)) {
            return null;
        }
        
        var reference = entityTable[handle.entityId];
        // Debug.Log(reference);
        
        if (handle.alive && !reference.entity) {
            var prefab = AssetManager.Get<GameObject>(reference.assetPath);
            var entity = Instantiate(prefab).GetComponent<Entity>();
            reference.entity = entity;
            entityIdReassignTable[entity] = handle.entityId;

        } else if (!handle.alive && reference.entity) {
            Destroy(reference.entity);
            return null;
        }
        
        return reference.entity;
    }

    private void PruneEntities() {
        foreach (var entity in entityTable.Values.ToList()) {
            if (!entity.alive) continue;
            if (entity.entity is PlayerCharacter) continue;
            
            if (!entity.entity.owner.summons.Contains(entity.entity)) {
                entityTable.Remove(entity.entityId);
                if (entity.entity) Destroy(entity.entity.gameObject);
            }
        }
    }

    public void HandlePlayerDeath(PlayerCharacter player) {
        extraGlobalStateFlags |= CharacterStateFlag.GLOBAL_PAUSE_TIMER | CharacterStateFlag.PAUSE_INPUT | CharacterStateFlag.PAUSE_GAUGE;
        AudioManager.inst.PlayAudioClip("cmn/announcer/battle/ko", gameObject, "active_announcer");

    }
    
    private void BeginDebugMatch() {
        var room = new LocalRoom(debugRoomConfig);
        room.players[0] = PlayerMatchData.CreateDebugPlayerData(0, "keyboard1", debugCharacterP1);
        room.players[1] = PlayerMatchData.CreateDebugPlayerData(1, "keyboard2", debugCharacterP2);
        room.stageData = debugStageData;
        room.bgmData = debugBgmData;
        RoomManager.inst.CreateRoom(room);
        
        PreloadResources();
        AudioManager.inst.PlayBGM(debugBgmData, gameObject, 1f, .2f);
        
        room.BeginMatch();
    }
    
    public void Serialize(StateSerializer serializer) {
        serializer.Put("pushboxCorrectionLock", pushboxCorrectionLock);
        serializer.Put("entityIdCounter", entityIdCounter);
        
        
        {
            // players
            var playersSerializer = new StateSerializer();
            foreach (var (k, v) in players) {
                var pth = new StateSerializer();
                pth.Put("_id", k);
                v.Serialize(pth);
                
                playersSerializer.Put(k.ToString(), pth.objects);
            }
            serializer.Put("players", playersSerializer.objects);
        }
    }
    
    public void Deserialize(StateSerializer serializer) {
        pushboxCorrectionLock = serializer.Get<bool>("pushboxCorrectionLock");
        entityIdCounter = serializer.Get<int>("entityIdCounter");

        {
            // players
            var playersSerializer = serializer.GetObject("players");
            foreach (var (k, pth) in playersSerializer.objects) {
                var player = players[int.Parse(k)];
                player.Deserialize(playersSerializer.GetObject(k));
            }
        }
    }
}


}
