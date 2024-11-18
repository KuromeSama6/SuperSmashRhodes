using System.Collections;
using UnityEngine;

namespace SuperSmashRhodes.Framework {
public class CoroutineHelper : AutoInitSingletonBehaviour<CoroutineHelper> {
    public static void RunCoroutine(IEnumerator coroutine) {
        inst.StartCoroutine(coroutine);
    }
}
}
