using SuperSmashRhodes.Battle.Serialization;

namespace SuperSmashRhodes.Battle {
/// <summary>
/// Represents a reference to an entity that is always valid, even if the entity is destroyed.
/// </summary>
public class EntityPtr : IStateSerializable {
    public int entityId { get; private set; }
    public Entity entity;
    public bool alive { get; private set; }
    
}
}
