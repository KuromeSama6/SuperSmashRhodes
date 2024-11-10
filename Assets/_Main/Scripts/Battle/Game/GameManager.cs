using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.Enums;
using Tomlyn;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SuperSmashRhodes.Battle.Game {
public class GameManager : SingletonBehaviour<GameManager> {
    [Title("References")]
    [Title("Camera")]
    public CinemachineCamera mainCamera;
    public CinemachineTargetGroup targetGroup;

    [Title("Scene")]
    public GameObject ground;
    
    [Title("Debug")]
    public GameObject characterPrefab;
        
    private PlayerInputManager inputManager;
    private Dictionary<int, PlayerCharacter> players = new();
    
    private IEnumerator Start() {
        inputManager = GetComponent<PlayerInputManager>();
        
        targetGroup.Targets.Clear();
        
        //TODO: Change debug
        CreatePlayer(0, "Keyboard", Keyboard.current);
        CreatePlayer(1, "Keyboard2", Keyboard.current);

        yield return new WaitForFixedUpdate();
        foreach (var player in players.Values) player.BeginLogic();
    }

    private void CreatePlayer(int index, string controlScheme, InputDevice device) {
        var input = PlayerInput.Instantiate(characterPrefab, controlScheme: controlScheme, pairWithDevice: device);
        var player = input.GetComponent<PlayerCharacter>();
        player.Init(index);
        player.OnRoundInit();
        player.name = "Player" + index;
        
        players[index] = player;
        targetGroup.AddMember(player.transform, 1, 0.5f);
    }
    
    public PlayerCharacter GetOpponent(PlayerCharacter player) {
        return players[player.playerIndex == 0 ? 1 : 0];
    }
}
}
