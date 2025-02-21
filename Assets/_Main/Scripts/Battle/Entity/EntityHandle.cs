using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Serialization;

namespace SuperSmashRhodes.Battle {
/// <summary>
/// Represents a reference to an entity that is always valid, even if the entity is destroyed.
/// </summary>
public class EntityHandle : IHandle {
    public int entityId { get; private set; }
    public Entity entity { get; private set; }
    public bool alive { get; private set; }
    
    public EntityHandle(Entity entity) {
        this.entity = entity;
        entityId = entity.entityId;
        alive = entity;
    }
    
    public object GetObject() {
        return GameManager.inst.GetEntity(entityId);
    }
    
    public void Serialize(StateSerializer serializer) {
        serializer.Serialize("entityId", entityId);
    }
    public void Deserialize(StateSerializer serializer) {
        entityId = serializer.Deserialize<int>("entityId");
    }
}
}
