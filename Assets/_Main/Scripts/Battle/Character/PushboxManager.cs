using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.Game;
using UnityEngine;
using UnityEngine.Events;

namespace SuperSmashRhodes.Battle {
public class PushboxManager : MonoBehaviour {
    [Title("References")]
    public BoxCollider2D pushbox;
    public BoxCollider2D forcedRepositonBox;
    
    public UnityEvent onGroundContact { get; } = new();
    public bool atWall => atLeftWall || atRightWall;
    public bool atLeftWall => player.side == EntitySide.LEFT && Mathf.Abs(player.transform.position.x - GameManager.inst.stageData.leftWallPosition) < 1f;
    public bool atRightWall => player.side == EntitySide.RIGHT && Mathf.Abs(player.transform.position.x - GameManager.inst.stageData.rightWallPosition) < 1f;
    
    public float pushboxSize => pushbox.size.x;
    private PlayerCharacter player;
    private bool pushboxCorrectionLock;
    
    private void Start() {
        player = GetComponentInParent<PlayerCharacter>();
    }

    private void Update() {
        // Debug.Log($"{player.name}: wall={atWall}, left={atLeftWall}, right={atRightWall}");
    }

    private void FixedUpdate() {
        pushboxCorrectionLock = false;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject == GameManager.inst.ground) onGroundContact.Invoke();
        
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (player == null || !player.logicStarted) return;
        
        bool isOpponent = player.opponent == other.GetComponentInParent<PlayerCharacter>();
        // Debug.Log($"trigger enter: {other.transform.parent.name}/{other.name}, isOpponent: {isOpponent}");
        bool isPushbox = other.GetComponent<PushboxManager>();

        if (isOpponent && isPushbox) {
            // correction
            AttemptPushboxCorrection();
        }
        
    }

    private void AttemptPushboxCorrection() {
        if (pushboxCorrectionLock) return;
        if (!player.airborne) return;
        float size = pushbox.size.x;
        pushboxCorrectionLock = true;
        // Debug.Log("pushbox cor 1r");

        var stageData = GameManager.inst.stageData;
        var nearestWall = player.opponent.side == EntitySide.LEFT ? stageData.leftWallPosition : stageData.rightWallPosition;
        float gap = Mathf.Abs(nearestWall - player.opponent.transform.position.x);
        // Debug.Log($"{player.name}: gap {gap}, req={size}");
        
        float direction;
        if (player.opponent.pushboxManager.atWall || gap < 1f) {
            // Debug.Log($"at wall, left={player.opponent.pushboxManager.atLeftWall}, right={player.opponent.pushboxManager.atRightWall}");
            direction = player.opponent.side == EntitySide.LEFT ? 1 : -1;

        } else {
            direction = player.side == EntitySide.LEFT ? 1 : -1;
        }
        
        float x = player.opponent.transform.position.x + ((size + 0.1f) * direction);
        player.transform.position = new Vector3(x, player.transform.position.y, player.transform.position.z);
    }

}
}
