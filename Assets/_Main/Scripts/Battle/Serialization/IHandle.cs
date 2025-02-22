namespace SuperSmashRhodes.Battle.Serialization {
/// <summary>
/// A handle represents a reference to an object that can be serialized and deserialized.
/// The fields within a handle should be
/// </summary>
public interface IHandle {
    object Resolve();
}

public struct DirectReferenceHandle : IHandle {
    private object obj;
    public DirectReferenceHandle(object obj) {
        this.obj = obj;
    }

    public object Resolve() {
        return obj;
    }
    public void Serialize(StateSerializer serializer) {
        serializer.Put("_obj", obj);
    }
    public void Deserialize(StateSerializer serializer) {
        this.obj = serializer.Get<object>("_obj");
    }
}
}
