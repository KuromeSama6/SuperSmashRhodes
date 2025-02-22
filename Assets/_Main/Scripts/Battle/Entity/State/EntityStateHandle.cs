using SuperSmashRhodes.Battle.Serialization;

namespace SuperSmashRhodes.Battle.State {
public struct EntityStateHandle : IHandle {
    private string id;
    private IHandle entity;
    
    public EntityStateHandle(Entity entity, string id) {
        this.entity = entity.GetHandle();
        this.id = id;
    }
    
    public void Serialize(StateSerializer serializer) {
        serializer.Put("id", id);
        serializer.Put("entity", entity);
    }
    public void Deserialize(StateSerializer serializer) {
        id = serializer.Get<string>("id");
        entity = serializer.Get<IHandle>("entity");
    }
    
    public object Resolve() {
        var entity = (Entity)this.entity.Resolve();
        return entity.states[id];
    }

    public override string ToString() {
        return $"EntityStateHandle({id})";
    }
}
}
