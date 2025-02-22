using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Serialization;

namespace SuperSmashRhodes.Battle {
/// <summary>
/// Represents a reference to an entity that is always valid, even if the entity is destroyed.
/// </summary>

public class EntityReference : IHandleSerializable {
    public string assetPath { get; private set; }
    public int entityId { get; private set; }
    public bool alive { get; set; }
    public Entity entity { get; set; }
    
    public EntityReference(Entity entity, int id) {
        this.entity = entity;
        assetPath = entity.assetPath;
        entityId = id;
        alive = entity;
    }
    
    public IHandle GetHandle() {
        return new EntityHandle(this);
    }

    public override string ToString() {
        return $"EntityReference({entityId}, alive={alive}, path={assetPath})";
    }
}

public struct EntityHandle : IHandle {
    public int entityId { get; private set; }
    public bool alive { get; private set; }
    
    public EntityHandle(Entity entity) {
        entityId = entity.entityId;
        alive = entity;
    }
    
    public EntityHandle(EntityReference entity) {
        entityId = entity.entityId;
        alive = entity.alive;
    }
    
    public object Resolve() {
        return GameManager.inst.ResolveEntity(this); 
    }

    public override string ToString() {
        return $"EntityHandle({entityId}, alive={alive})";
    }
}
}
