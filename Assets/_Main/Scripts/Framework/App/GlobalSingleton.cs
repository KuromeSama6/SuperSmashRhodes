using System;

namespace SuperSmashRhodes.Framework {
public abstract class GlobalSingleton<T> where T: new() {
    public static T inst => _instance.Value;
    private static Lazy<T> _instance = new(() => new T());
}
}
