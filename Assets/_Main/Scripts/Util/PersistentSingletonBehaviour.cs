using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tomlyn {
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
