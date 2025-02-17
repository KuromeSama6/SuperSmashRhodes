using System;

namespace SuperSmashRhodes.Framework {
[AttributeUsage(AttributeTargets.Class)]
public class NamedTokenAttribute : Attribute {
    public string name { get; private set; }
    public int priority { get; private set; }
    public NamedTokenAttribute(string name, int priority = 0) {
        this.name = name;
        this.priority = priority;
    }
}
}
