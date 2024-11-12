using System;
using Spine.Unity;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class EntityBoundingBox : MonoBehaviour {
    public BoundingBoxType type { get; set; }
    public Entity owner { get; set; }
    
    private BoundingBoxFollower bbFollower;
    public PolygonCollider2D collider { get; private set; }

    private void Start() {
        bbFollower = GetComponent<BoundingBoxFollower>();

        bbFollower.isTrigger = true;
    }

    private void FixedUpdate() {
        if (!collider) {
            collider = GetComponent<PolygonCollider2D>();
            if (!collider) return;
            
            collider.isTrigger = true;
            collider.hideFlags = HideFlags.None;
        }
        
        SkeletonUtility.SetColliderPointsLocal(collider, bbFollower.Slot, bbFollower.CurrentAttachment);
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
        if (bb == null || bb.owner == owner) return;

        // Debug.Log($"{owner.name}: {name} hit {bb}");
         
        owner.HandleEntityInteraction(this, bb, new() {
            point = other.ClosestPoint(transform.position)
        });
    }
    

}

[Flags]
public enum BoundingBoxType {
    PUSHBOX = 1 << 0,
    HITBOX = 1 << 1,
    HURTBOX = 1 << 2,
    
    CHR_MAIN_PUSHBOX = PUSHBOX | HURTBOX
}

public struct EntityBBInteractionData {
    public Vector2 point;
}
}
