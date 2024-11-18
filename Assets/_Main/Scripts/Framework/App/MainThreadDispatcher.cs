using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace SuperSmashRhodes.Framework {
public class MainThreadDispatcher : AutoInitSingletonBehaviour<MainThreadDispatcher> {
    private static SynchronizationContext mainThreadContext;

    private Queue<Action> mainThreadActions = new();

    protected override void Awake() {
        base.Awake();
        
        mainThreadContext = SynchronizationContext.Current;
    }

    // Enqueue an action to run on the main thread
    public static void RunOnMain(Action action) {
        if (mainThreadContext == null)
            throw new NotSupportedException("MainThreadDispatcher not initialized.");
            
        inst.EnqueueAction(action);
    }

    private void EnqueueAction(Action action) {
        lock (mainThreadActions) {
            mainThreadActions.Enqueue(action);
        }
    }

    // Execute all actions queued for the main thread
    private void Update() {
        lock (mainThreadActions) {
            while (mainThreadActions.Count > 0) {
                Action action = mainThreadActions.Dequeue();
                action?.Invoke();
            }
        }
    }
}
}
