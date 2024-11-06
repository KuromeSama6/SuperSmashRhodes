using System.Collections;
using UnityEngine;

namespace SuperSmashRhodes.Util {
public class CoroutineWrapper<T> : IEnumerator {
    public T result { get; private set; }
    public bool hasResult { get; private set; }

    private IEnumerator coroutine;

    public CoroutineWrapper(IEnumerator coroutine) {
        this.coroutine = coroutine;
    }

    public bool MoveNext() {
        if (hasResult) return false;
        if (coroutine.MoveNext()) {
            return true;
        }

        if (coroutine.Current is CoroutineResult<T> end) {
            result = end.result;
            hasResult = true;
        }

        return false;
    }

    public void Reset() {
        coroutine.Reset();
    }

    public object Current => coroutine.Current;

}

public class CoroutineResult<T> {
    public T result { get; }
    public CoroutineResult(T result) {
        this.result = result;
    }
}

}
