using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.Stage;
using SuperSmashRhodes.Framework;
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
    
    [Title("Debug")]
    public GameObject p1Prefab;
    public GameObject p2Prefab;
        
    private PlayerInputManager inputManager;
    private Dictionary<int, PlayerCharacter> players = new();
    private bool pushboxCorrectionLock = false;
    private readonly Dictionary<int, Entity> entityTable = new();
    private int entityIdCounter;
    
    private IEnumerator Start() {
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
        var x = Mathf.Clamp(position.x, stageData.leftWallPosition, stageData.rightWallPosition);
        return new Vector3(x, position.y, position.z);
    }

    private void Update() {
        foreach (var target in targetGroup.Targets) {
            var player = GetPlayer(targetGroup.Targets.IndexOf(target));
            target.Object = player.stateFlags.HasFlag(CharacterStateFlag.CAMERA_FOLLOW_BONE) ? player.cameraFollowSocket : player.transform;
            target.Weight = player.cameraGroupWeight;
        }
    }

    private void FixedUpdate() {
        pushboxCorrectionLock = false;
    }

    public void AttemptPushboxCorrection() {
        if (pushboxCorrectionLock) return;
        pushboxCorrectionLock = true;
        var p1 = players[0];
        var p2 = players[1];
        
        // find the character that needs to move
        PlayerCharacter target;
        if (p1.atWall) target = p2;
        else if (p2.atWall) target = p1;
        else {
            // prioritize wall distance
            // Debug.Log($"p1 {p1.wallDistance} p2 {p2.wallDistance}");
            if (p1.wallDistance < p2.wallDistance) target = p2;
            else target = p1;
        }
        
        float size = target.pushboxManager.pushboxSize;
        float offset = (target.transform.position.x <= target.opponent.transform.position.x ? -1 : 1) * (size + .1f);
        target.transform.position += new Vector3(offset, 0, 0);

    }
    
    public int RegisterEntity(Entity entity) {
        var id = entityIdCounter++;
        entityTable[id] = entity;
        return id;
    }
}


}
