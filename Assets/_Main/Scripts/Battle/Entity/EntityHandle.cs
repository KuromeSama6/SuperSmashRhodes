using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Serialization;

namespace SuperSmashRhodes.Battle {
/// <summary>
/// Represents a reference to an entity that is always valid, even if the entity is destroyed.
/// </summary>
public struct EntityHandle : IHandle {
    public string assetPath { get; private set; }
    public int entityId { get; private set; }
    public bool alive { get; private set; }
    
    public EntityHandle(Entity entity) {
        assetPath = entity.assetPath;
        entityId = entity.entityId;
        alive = entity;
    }
    
    public object Resolve() {
        return GameManager.inst.ResolveEntity(this); 
    }
    
    public void Serialize(StateSerializer serializer) {
        serializer.Put("entityId", entityId);
    }
    public void Deserialize(StateSerializer serializer) {
        entityId = serializer.Get<int>("entityId");
    }

    public override string ToString() {
        return $"EntityHandle({entityId})";
    }
}
}
