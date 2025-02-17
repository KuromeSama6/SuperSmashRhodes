using System;

namespace SuperSmashRhodes.Battle.Animation {
[AttributeUsage(AttributeTargets.Method)]
public class AnimationEventHandlerAttribute : Attribute {
    public readonly string name;
    public AnimationEventHandlerAttribute(string name) {
        this.name = name;
    }
}

public struct AnimationEventData {
    public readonly string[] args;
    public readonly int integerValue;
    public readonly float floatValue;
    public readonly string audioPath;
    
    public AnimationEventData(string str, int integerValue, float floatValue, string audioPath) {
        args = str.Split();
        this.integerValue = integerValue;
        this.floatValue = floatValue;
        this.audioPath = audioPath;
    }
    
    public string GetArg(int index, string def = "") {
        if (index < args.Length) return args[index];
        return def;
    }
}
}
