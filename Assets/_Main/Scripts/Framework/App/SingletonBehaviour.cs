using UnityEngine;

namespace SuperSmashRhodes.Framework {
public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
    public static T inst { get; protected set; }

    // Start is called before the first playhead update
    protected virtual void Awake() {
        inst = this as T;
    }
}
}
