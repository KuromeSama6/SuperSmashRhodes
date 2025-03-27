using System;
using Spine.Unity;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Serialization;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class EntityBoundingBox : MonoBehaviour, IEntityBoundingBox, IEngineUpdateListener {
    public BoundingBoxType type { get; set; }
    public Entity entity { get; set; }
    public PlayerCharacter owningPlayer { get; set; }
    private BoundingBoxFollower bbFollower;
    public PolygonCollider2D collider { get; private set; }
    public Collider2D box => collider;

    private void Start() {
        bbFollower = GetComponent<BoundingBoxFollower>();
        bbFollower.isTrigger = true;
        owningPlayer = entity.owner;
    }

    public void EngineUpdate() {
        if (!entity.logicStarted) return;
        
        if (!collider) {
            collider = GetComponent<PolygonCollider2D>();
            if (!collider) return;
            
            collider.isTrigger = true;
            collider.hideFlags = HideFlags.None;
        }
        
        SkeletonUtility.SetColliderPointsLocal(collider, bbFollower.Slot, bbFollower.CurrentAttachment);

        if (type == BoundingBoxType.HITBOX) {
            enabled = entity.activeState != null && entity.activeState.enableHitboxes;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        OnTrigger(other);
    }

    private void OnTriggerStay2D(Collider2D other) {
        OnTrigger(other);
    }

    private void OnTrigger(Collider2D other) {
        if (!other.isTrigger) return;
        
        // handle
        var bb = other.gameObject.GetComponent<EntityBoundingBox>();
        if (bb == null || bb.entity == entity) return;

        // Debug.Log($"{owningPlayer.name}: {name} hit {bb}");
         
        entity.HandleEntityInteraction(this, bb, new() {
            point = other.ClosestPoint(transform.position)
        });
    }

    public void EnsureAttachment() {
        enabled = true;
        bbFollower.MatchAttachment(bbFollower.Slot.Attachment);
    }

    public void ManualUpdate() {
        
    }
}

public struct EntityBBInteractionData {
    public Vector2 point;
}
}
