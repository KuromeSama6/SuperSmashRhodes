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
    [FormerlySerializedAs("p2Prfab")]
    public GameObject p2Prefab;
        
    private PlayerInputManager inputManager;
    private Dictionary<int, PlayerCharacter> players = new();
    
    private IEnumerator Start() {
        inputManager = GetComponent<PlayerInputManager>();
        
        targetGroup.Targets.Clear();
        
        //TODO: Change debug
        CreatePlayer(0, p1Prefab, "Keyboard", Keyboard.current);
        CreatePlayer(1, p2Prefab,"Keyboard2", Keyboard.current);

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

    private void CreatePlayer(int index, GameObject prefab, string controlScheme, InputDevice device) {
        var input = PlayerInput.Instantiate(prefab, controlScheme: controlScheme, pairWithDevice: device);
        var player = input.GetComponent<PlayerCharacter>();
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
}
}
