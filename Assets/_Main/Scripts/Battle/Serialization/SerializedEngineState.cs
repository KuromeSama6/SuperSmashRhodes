using System.Text;

namespace SuperSmashRhodes.Battle.Serialization {
/// <summary>
/// Represents a snapshot of the game state that can be serialized and deserialized.
/// </summary>
public class SerializedEngineState {
    public int frame { get; private set; }
    public StateSerializer serializer { get; private set; }

    public SerializedEngineState(int frame, StateSerializer serializer) {
        this.frame = frame;
        this.serializer = serializer;
    }

    public string DumpToString() {
        StringBuilder sb = new();
        sb.AppendLine("State Serialization Dump");
        sb.AppendLine("----");
        sb.AppendLine($"Parent type: {serializer.objects.parentType}");

        foreach (var key in serializer.objects.Keys) {
            var obj = serializer.objects[key];
            FormatSerialized(key, obj, sb);
        }
        
        return sb.ToString();
    }
    
    
    private void FormatSerialized(string key, object obj, StringBuilder sb, int level = 0) {
        for (int i = 0; i < level; i++) {
            sb.Append("    ");
        }

        if (obj is DirectReferenceHandle directReferenceHandle) {
            sb.Append($"{key}: <DirectRef> ({directReferenceHandle.Resolve()})");
            
        } else if (obj is IHandle handle) {
            sb.Append($"{key}: <Handle> ({handle})");
            
        } else if (obj is SerializedDictionary dict) {
            sb.AppendLine($"{key}: <Dict> ({dict.parentType}) ({dict.Count})");
            foreach (var dictKey in dict) {
                FormatSerialized(dictKey.Key, dictKey.Value, sb, level + 1);
            }
            
        } else if (obj is SerializedCollection list) {
            sb.AppendLine($"{key}: <List> ({list.parentType}) ({list.Count})");
            int i = 0;
            foreach (var d in list) {
                FormatSerialized($"[{i}]", d, sb, level + 1);
                i++;
            }
            
        } else {
            sb.Append($"{key}: {obj} ({(obj == null ? "<null>" : obj.GetType())})");
        }
        sb.Append("\n");
    }
}
}
