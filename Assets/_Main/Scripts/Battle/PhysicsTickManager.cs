using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class PhysicsTickManager : SingletonBehaviour<PhysicsTickManager> {
    public int globalFreezeFrames { get; set; }

    private int scheduledDelay;
    private int scheduledFreezeFrames;
    private List<Action> queuedActions = new List<Action>();
    
    private void Start() { 
        Physics2D.simulationMode = SimulationMode2D.Script;
    }

    private void Update() {
        
    }

    private void FixedUpdate() { 
        if (scheduledDelay > 0) {
            --scheduledDelay;
            if (scheduledDelay == 0) {
                globalFreezeFrames += scheduledFreezeFrames;
                scheduledFreezeFrames = 0;
            }
        }
        
        if (globalFreezeFrames > 0) {
            // Debug.Log(globalFreezeFrames);
            --globalFreezeFrames;
            return;
        }

        // Debug.Log($"{Time.frameCount} invoked");
        if (scheduledDelay == 0) {
         
            foreach (var action in queuedActions) action.Invoke();
            queuedActions.Clear();   
        }
        
        Physics2D.Simulate(Time.fixedDeltaTime);
    }
    
    public void Schedule(int delay, int freezeFrames) {
        // Debug.Log("schedule, " + freezeFrames);
        scheduledDelay = delay;
        scheduledFreezeFrames = freezeFrames;
    }
    
    public void Queue(Action action) {
        queuedActions.Add(action);
    }
}

}
