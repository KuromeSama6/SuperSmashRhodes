using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State {
public abstract class EntityState : NamedToken {
    public Entity owner { get; private set; }
    public EntityStateData stateData { get; private set; }
    public bool active { get; private set; }
    public int frame { get; private set; }
    public abstract EntityStateType type { get; }
    public abstract int inputPriority { get; }
    public virtual bool mayEnterState => true;
    
    private int interruptFrames;
    private int scheduledPauseAnimationFrames;
    private IEnumerator routine;
    
    public EntityState(Entity owner) {
        this.owner = owner;
    }

    private void Init() {
        OnStateBegin();
        routine = MainRoutine();
    }

    public void BeginState() {
        stateData = new();
        frame = 0;
        interruptFrames = 0;
        scheduledPauseAnimationFrames = 0;
        Init();
        active = true;
    }
    
    public void TickState() {
        if (!active) return;
        ++frame;
        
        // scheduled animation
        if (scheduledPauseAnimationFrames > 0) {
            --scheduledPauseAnimationFrames;
            
        } else {
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
            interruptFrames += Mathf.Max(framesSkipped - 1, 0);
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
    
    protected void SetScheduledPauseAnimationFrames(int frames) {
        scheduledPauseAnimationFrames = frames;
    }
    
    // Virtual methods / Events
    protected virtual void OnStateBegin() { }
    // Abstract methods
    public abstract bool IsInputValid(InputBuffer buffer);
    public abstract IEnumerator MainRoutine();
}

public abstract class CharacterState : EntityState {
    public PlayerCharacter player { get; private set; }
    protected PlayerCharacter opponent => player.opponent;
    
    public CharacterState(Entity owner) : base(owner) {
        player = (PlayerCharacter)owner;
    }
}
}
