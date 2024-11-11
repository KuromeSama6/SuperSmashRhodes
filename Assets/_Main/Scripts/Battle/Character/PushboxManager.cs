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
    public float pushboxSize => pushbox.size.x;

    private bool atLeftWall, atRightWall;
    private PlayerCharacter player;
    private bool pushboxCorrectionLock;
    
    private void Start() {
        player = GetComponentInParent<PlayerCharacter>();
    }

    private void Update() {
    }

    private void FixedUpdate() {
        pushboxCorrectionLock = false;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject == GameManager.inst.ground) onGroundContact.Invoke();

        if (player == null) return;   
        var side = player.side;
        if (other.gameObject == GameManager.inst.rightWall && side == EntitySide.RIGHT) {
            atRightWall = true;
            // Debug.Log("at l wall");
        }
        if (other.gameObject == GameManager.inst.leftWall && side == EntitySide.LEFT) {
            atLeftWall = true;
            // Debug.Log("at r wall");
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject == GameManager.inst.leftWall || other.gameObject == GameManager.inst.rightWall) {
            atLeftWall = atRightWall = false;
        }
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

        var nearestWall = player.opponent.side == EntitySide.LEFT ? GameManager.inst.leftWall : GameManager.inst.rightWall;
        float gap = Mathf.Abs(nearestWall.transform.position.x - player.opponent.transform.position.x);
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
