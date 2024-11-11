using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.Enums;
using Unity.Cinemachine;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace SuperSmashRhodes.Battle.Game {
public class GameManager : SingletonBehaviour<GameManager> {
    [Title("References")]
    [Title("Camera")]
    public CinemachineCamera mainCamera;
    public CinemachineTargetGroup targetGroup;

    [Title("Scene")]
    public GameObject ground;
    public GameObject leftWall, rightWall;
    
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
}
}
