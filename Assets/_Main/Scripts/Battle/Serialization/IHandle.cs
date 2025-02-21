namespace SuperSmashRhodes.Battle.Serialization {
/// <summary>
/// A handle represents a reference to an object that can be serialized and deserialized.
/// The fields within a handle should be
/// </summary>
public interface IHandle : IStateSerializable {
    object GetObject();
}

public class DirectReferenceHandle : IHandle {
    private object obj;
    public DirectReferenceHandle(object obj) {
        this.obj = obj;
    }

    public object GetObject() {
        return obj;
    }
    public void Serialize(StateSerializer serializer) {
        serializer.Serialize("_obj", obj);
    }
    public void Deserialize(StateSerializer serializer) {
        this.obj = serializer.Deserialize<object>("_obj");
    }
}
}
