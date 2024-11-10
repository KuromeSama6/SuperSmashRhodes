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

    private PlayerCharacter player;
    
    private void Start() {
        player = GetComponentInParent<PlayerCharacter>();
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject == GameManager.inst.ground)
            onGroundContact.Invoke();
    }

    public void OnTriggerEnter2D(Collider2D other) {
        bool isOpponent = player.opponent == other.GetComponentInParent<PlayerCharacter>();
        // Debug.Log($"trigger enter: {other.transform.parent.name}/{other.name}, isOpponent: {isOpponent}");
        bool isPushbox = other.GetComponent<PushboxManager>();

        if (isOpponent && isPushbox) {
            // correction
            AttemptPushboxCorrection();
        }
        
    }

    private void AttemptPushboxCorrection() {
        if (!player.airborne) return;
        float size = pushbox.size.x;
        float direction = player.side == EntitySide.LEFT ? 1 : -1;
        
        float x = player.opponent.transform.position.x + ((size + 0.1f) * direction);
        player.transform.position = new Vector3(x, player.transform.position.y, player.transform.position.z);
    }

}
}
