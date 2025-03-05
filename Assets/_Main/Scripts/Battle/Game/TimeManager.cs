using System;
using System.Collections.Generic;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Framework;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class TimeManager : SingletonBehaviour<TimeManager>, IManualUpdate, IAutoSerialize {
    public int globalFreezeFrames { get; set; }

    private int scheduledDelay;
    private int scheduledFreezeFrames;
    private List<Action> queuedActions = new List<Action>();
    
    private void Start() { 
        Physics2D.simulationMode = SimulationMode2D.Script;
    }

    public void ManualUpdate() {
        
    }

    public void EngineUpdate() { 
        if (scheduledDelay > 0) {
            --scheduledDelay;
            if (scheduledDelay == 0) {
                globalFreezeFrames += scheduledFreezeFrames;
                scheduledFreezeFrames = 0;
            }
        } else {
            globalFreezeFrames += scheduledFreezeFrames;
            scheduledFreezeFrames = 0;
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
        
        // Debug.Log("simulate");
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
    public void Serialize(StateSerializer serializer) {
        serializer.Put("globalFreezeFrames", globalFreezeFrames);
        serializer.Put("scheduledDelay", scheduledDelay);
        serializer.Put("scheduledFreezeFrames", scheduledFreezeFrames);
        serializer.PutList("queuedActions", queuedActions);
    }
    
    public void Deserialize(StateSerializer serializer) {
        globalFreezeFrames = serializer.Get<int>("globalFreezeFrames");
        scheduledDelay = serializer.Get<int>("scheduledDelay");
        scheduledFreezeFrames = serializer.Get<int>("scheduledFreezeFrames");
        serializer.GetList("queuedActions", queuedActions);
    }
}

}
