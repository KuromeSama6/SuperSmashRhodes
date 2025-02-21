using SuperSmashRhodes.Battle.Serialization;

namespace SuperSmashRhodes.Battle.State {
public class EntityStateHandle : IHandle {
    private string id;
    private IHandle entity;
    
    public EntityStateHandle(Entity entity, string id) {
        this.entity = entity.GetHandle();
        this.id = id;
    }
    
    public void Serialize(StateSerializer serializer) {
        serializer.Serialize("id", id);
        serializer.Serialize("entity", entity);
    }
    public void Deserialize(StateSerializer serializer) {
        id = serializer.Deserialize<string>("id");
        entity = serializer.Deserialize<IHandle>("entity");
    }
    
    public object GetObject() {
        var entity = (Entity)this.entity.GetObject();
        return entity.states[id];
    }
}
}
