using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State {
public abstract class EntityState {
    public string id { get; }
    public Entity owner { get; private set; }
    public EntityStateData stateData { get; private set; }
    public bool active { get; private set; }
    public int frame { get; private set; }
    public abstract EntityStateType type { get; }
    public abstract int inputPriority { get; }
    public virtual bool mayEnterState => true;
    
    private int interruptFrames;
    private int scheduledAnimationFrames;
    private IEnumerator routine;
    
    public EntityState(Entity owner, string id) {
        this.id = id;
        this.owner = owner;
    }

    private void Init() {
        OnStateInit();
        routine = MainRoutine();
    }

    public void BeginState() {
        stateData = new();
        frame = 0;
        interruptFrames = 0;
        scheduledAnimationFrames = 0;
        Init();
        active = true;
    }
    
    public void TickState() {
        if (!active) return;
        ++frame;
        
        // scheduled animation
        if (scheduledAnimationFrames > 0) {
            --scheduledAnimationFrames;
            owner.animation.Tick();
        }
        
        // interrupt frames
        if (interruptFrames > 0) {
            --interruptFrames;
            return;
        }
        
        if (routine.MoveNext()) {
            var current = routine.Current;
            HandleRoutineReturn(current);
            
        } else {
            EndState();
        }
    }

    public void EndState() {
        active = false;
        
    }
    
    private void HandleRoutineReturn(object obj) {
        if (obj == null) return;
        
        // interrupt frames
        if (obj is int framesSkipped) {
            interruptFrames += framesSkipped;
            return;
        }

        if (obj is EntityStateYieldInstruction yieldInstruction) {
            yieldInstruction.Execute(stateData);
            return;
        }
        
    }
    
    // Member methods
    protected void AddCancelOption(string stateName) {
        if (!owner.states.TryGetValue(stateName, out var state))
            throw new KeyNotFoundException($"State {stateName} not found");
            
        stateData.cancelOptions.Add(state);
    }
    
    protected void AddCancelOption(EntityStateType flag) {
        stateData.cancelFlag |= flag;
    }

    protected bool RevalidateInput() {
        return IsInputValid(GetCurrentInputBuffer());
    }
    
    protected InputBuffer GetCurrentInputBuffer() {
        if (!(owner is PlayerCharacter player))
            throw new NotSupportedException();

        return player.inputModule.localBuffer;
    }

    protected void CancelInto(string name) {
        if (!owner.states.TryGetValue(name, out var state))
            throw new KeyNotFoundException($"State {name} not found");
        
        EndState();
        owner.BeginState(state);
    }
    
    protected void SetScheduledAnimationFrames(int frames, bool tickImmediate = true) {
        if (tickImmediate) owner.animation.Tick();
        scheduledAnimationFrames = frames;
    }
    
    // Virtual methods / Events
    protected virtual void OnStateInit() { }
    // Abstract methods
    public abstract bool IsInputValid(InputBuffer buffer);
    public abstract IEnumerator MainRoutine();
}
}
