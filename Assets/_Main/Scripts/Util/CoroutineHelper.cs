using System.Collections;
using UnityEngine;

namespace SuperSmashRhodes.Util {
public class CoroutineHelper : AutoInitSingletonBehaviour<CoroutineHelper> {
    public static void RunCoroutine(IEnumerator coroutine) {
        inst.StartCoroutine(coroutine);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init() {
        InitInternal();
    }
}
}
