using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SuperSmashRhodes.Battle {
public class PushboxManager : MonoBehaviour, IManualUpdate {
    [Title("References")]
    [Tooltip("The physics collider that collides with the ground and other players only.")]
    public BoxCollider2D physicsBox;
    [Tooltip("The trigger collider used for position correction and width measurement.")]
    public BoxCollider2D correctionBox;
    [Tooltip("The trigger collider used for wall detection.")]
    public BoxCollider2D wallDetectionBox;
    [Tooltip("The physics collider used for non-player interactions only.")]
    public BoxCollider2D interactionPhysicsBox;
    
    private readonly List<Collider2D> contactBuffer = new();
    
    public UnityEvent onGroundContact { get; } = new();
    public bool grounded { get; private set; } = true;
    
    public bool atWall => atLeftWall || atRightWall; 
    public bool atLeftWall {
        get {
            if (player.wallDistance <= .02f) return true;
            if (player.side == EntitySide.LEFT) {
                wallDetectionBox.GetContacts(contactBuffer);
                return contactBuffer.Contains(GameManager.inst.leftWall.GetComponent<Collider2D>());
            } else {
                return false;
            }
        }
    }
    public bool atRightWall {
        get {
            if (player.wallDistance <= .02f) return true;
            if (player.side == EntitySide.RIGHT) {
                wallDetectionBox.GetContacts(contactBuffer);
                return contactBuffer.Contains(GameManager.inst.rightWall.GetComponent<Collider2D>());
            } else {
                return false;
            }
        }
    }
    
    private PlayerCharacter player;
    private bool pushboxCorrectionLock;
    
    private void Start() {
        player = GetComponentInParent<PlayerCharacter>();
    }

    private void Update() {
        // Debug.Log($"{player.name}: wall={atWall}, left={atLeftWall}, right={atRightWall}");
    }

    public void ManualFixedUpdate() {
        pushboxCorrectionLock = false;
        if (!player || !player.opponent) return;

        {
            // pushboxes
            if (player.activeState != null) {
                physicsBox.enabled = !player.activeState.stateData.physicsPushboxDisabled;
            }
        }

        {
            // ground check
            // Debug.Log($"{player.playerIndex} {player.transform.position.y}");
            var isGrounded = player.transform.position.y <= .03f;
            // if (player.playerIndex == 1) Debug.Log($"oldstate {grounded} gnd {isGrounded} pos {player.transform.position.y}");
            if (isGrounded != grounded) {
                grounded = isGrounded;
                if (grounded) {
                    onGroundContact.Invoke();
                    // Debug.Log("grounded");
                }
            }
        }

        {
            // pushbox correction
            var buf = new List<Collider2D>();
            correctionBox.GetContacts(buf);
            var opponent = player.opponent;
            // if (player.playerIndex == 0) Debug.Log(opponent.rb.linearVelocityY < .000001f);
            if (opponent.transform.position.y >= .1f && buf.Contains(opponent.pushboxManager.physicsBox) && opponent.rb.linearVelocityY <= .000001f) {
                // Debug.Log($"correct vel={opponent.rb.linearVelocity}");
                GameManager.inst.AttemptPushboxCorrection(opponent, player);
            }
        }
    }
    public void ManualUpdate() {
    }
}
}
