using System;
using UnityEngine;

namespace SuperSmashRhodes.Util {
public abstract class AutoInitSingletonBehaviour<T> : PersistentSingletonBehaviour<T> where T : AutoInitSingletonBehaviour<T> {
    protected virtual string objectName => $"{GetType().Name}.Singleton";
    
    protected static void InitInternal() {
        if (inst != null) throw new NotSupportedException("Instance already exists");
        
        var go = new GameObject();
        var comp = go.AddComponent<T>();
        go.name = comp.objectName;
    }
}
}
