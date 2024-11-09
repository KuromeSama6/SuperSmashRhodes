using System;

namespace SuperSmashRhodes.Framework {
[AttributeUsage(AttributeTargets.Class)]
public class NamedTokenAttribute : Attribute {
    public string name { get; private set; }
    public NamedTokenAttribute(string name) {
        this.name = name;
    }
}
}
