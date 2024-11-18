using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperSmashRhodes.Framework {
public abstract class PersistentSingletonBehaviour<T> : SingletonBehaviour<T> where T : MonoBehaviour {
    protected override void Awake() {
        if (inst != null && inst != this) {
            Destroy(gameObject);
            return;
        }

        inst = this as T;
        DontDestroyOnLoad(gameObject);
    }
}
}
