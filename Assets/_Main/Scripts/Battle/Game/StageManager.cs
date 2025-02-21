using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Stage;
using SuperSmashRhodes.Framework;
using UnityEngine;

namespace SuperSmashRhodes.Scripts {
public class StageManager : SingletonBehaviour<StageManager>, IManualUpdate {
    [Title("References")]
    public GameObject leftWall;
    public GameObject rightWall;

    private StageData stageData => GameManager.inst.stageData;
    public float wallDistance => Mathf.Abs(rightWall.transform.position.x - leftWall.transform.position.x) - leftWall.GetComponent<BoxCollider2D>().size.x;
    public PlayerCharacter p1 => GameManager.inst.GetPlayer(0);
    public PlayerCharacter p2 => GameManager.inst.GetPlayer(1);
    
    private void Start() {
        
    }

    public void ManualUpdate() {
        
    }

    public void ManualFixedUpdate() {
        var centerPoint = (p1.transform.position.x + p2.transform.position.x) / 2f;
        if (p1.activeState == null || p2.activeState == null) return;
        
        var modifier = p1.activeState.stateData.midscreenWallDistanceModifier + p2.activeState.stateData.midscreenWallDistanceModifier;
        var distanceMax = stageData.midscreenWallDistanceMax + modifier;
        
        var leftWallTarget = centerPoint - distanceMax / 2f;
        var rightWallTarget = centerPoint + distanceMax / 2f;

        leftWall.transform.position = new(Mathf.Max(leftWallTarget, -stageData.cornerWallX), leftWall.transform.position.y, leftWall.transform.position.z);
        rightWall.transform.position = new(Mathf.Min(rightWallTarget, stageData.cornerWallX), rightWall.transform.position.y, rightWall.transform.position.z);
    }

}
}
