using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
[RequireComponent(typeof(Collider2D))]
public class ExplicitBoundingBox : MonoBehaviour, IEntityBoundingBox {
    [Title("Properties")]
    public BoundingBoxType boxType;
    public Collider2D colliderBox;
    public bool initialState = true;
    
    public BoundingBoxType type => boxType;
    public Collider2D box => colliderBox;
    public PlayerCharacter owningPlayer { get; set; }

    private Entity entity;
    
    public bool boxEnabled {
        get => box.enabled;
        set => box.enabled = value;
    }
    
    private void Start() {
        boxEnabled = initialState;
        entity = GetComponentInParent<Entity>();
        owningPlayer = entity.owner;
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
        if (!entity) return;
        
        entity.HandleEntityInteraction(this, bb, new() {
            point = other.ClosestPoint(transform.position)
        });
    }
    
    public void EnsureAttachment() {
    }
}
}
