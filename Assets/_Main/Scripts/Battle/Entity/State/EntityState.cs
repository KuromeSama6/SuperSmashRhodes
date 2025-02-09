using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Spine;
using SuperSmashRhodes.Battle.Animation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;
using UnityEngine.Events;

namespace SuperSmashRhodes.Battle.State {
public abstract class EntityState : NamedToken {
    public Entity entity { get; private set; }
    public EntityStateData stateData { get; private set; }
    public bool active { get; private set; }
    public int frame { get; private set; }
    public abstract EntityStateType type { get; }
    public virtual bool mayEnterState => true;
    public virtual bool fullyInvincible => false;
    public virtual bool isSelfCancellable => false;
    public virtual bool enableHitboxes => true;

    public UnityEvent onStateEnd { get; } = new();
    
    private int interruptFrames;
    private int scheduledPauseAnimationFrames;
    private IEnumerator routine;
    private readonly Dictionary<string, List<MethodInfo>> animationEventHandlers = new();
    
    public EntityState(Entity entity) {
        this.entity = entity;
        
        // Animation event handlers
        {
            foreach (var method in GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                var attr = method.GetCustomAttribute<AnimationEventHandlerAttribute>();
                if (attr != null) {
                    if (!animationEventHandlers.TryGetValue(attr.name, out var list)) {
                        list = new();
                        animationEventHandlers[attr.name] = list;
                    }
                    list.Add(method);
                }
            }
            // Debug.Log($"State {id} has {animationEventHandlers.Count} animation event handlers");
        }
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

        // Debug.Log($"frames: {PhysicsTickManager.inst.globalFreezeFrames}"); 
        OnTick();
        
        // scheduled animation
        if (entity.shouldTickAnimation) {
            if (scheduledPauseAnimationFrames > 0) {
                --scheduledPauseAnimationFrames;
            
            } else {
                entity.animation.Tick();
            }   
        }
        
        // interrupt frames
        if (!entity.shouldTickState) return;
        
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
        OnStateEnd();
        onStateEnd.Invoke();
    }

    public void FastForward(int frames) {
        for (var i = 0; i < frames; ++i) {
            TickState();
        }
    }
    
    public void HandleAnimationEvent(string name, AnimationEventData data) {
        if (animationEventHandlers.TryGetValue(name, out var method)) {
            foreach (var m in method) {
                if (m.GetParameters().Length == 1) {
                    m.Invoke(this, new object[]{data});
                } else {
                    m.Invoke(this, null);
                }
            }
        }
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
        if (!entity.states.TryGetValue(stateName, out var state))
            throw new KeyNotFoundException($"State {stateName} not found");
            
        stateData.cancelOptions.Add(state);
    }
    
    protected void AddCancelOption(EntityStateType flag) {
        stateData.cancelFlag |= flag;
    }

    protected void RemoveCancelOption(EntityStateType flag) {
        stateData.cancelFlag &= ~flag;
    }
    
    protected void RemoveCancelOption(string stateName) {
        if (!entity.states.TryGetValue(stateName, out var state))
            throw new KeyNotFoundException($"State {stateName} not found");
            
        stateData.cancelOptions.Remove(state);
    }
    
    protected void CancelInto(string name) {
        if (!entity.states.TryGetValue(name, out var state))
            throw new KeyNotFoundException($"State {name} not found");
        
        EndState();
        entity.BeginState(state);
    }
    
    protected void SetScheduledPauseAnimationFrames(int frames) {
        scheduledPauseAnimationFrames = frames;
    }
    
    // Virtual methods / Events
    protected virtual void OnTick() { }
    protected virtual void OnStateBegin() { }
    protected virtual void OnStateEnd() {}
    // Abstract methods
    public abstract IEnumerator MainRoutine();
    
    // Handlers
    [AnimationEventHandler("std/ApplyCinematicDamage")]
    public virtual void OnApplyCinematicDamage(AnimationEventData data) {
        if (entity is PlayerCharacter player) {
            player.opponent.ApplyDamage(data.integerValue, null, DamageSpecialProperties.REAL_DAMAGE | DamageSpecialProperties.SKIP_REGISTER);
        }
    }
    
    [AnimationEventHandler("std/PlaySound")]
    public virtual void OnPlaySound(AnimationEventData data) {
        var path = data.args[0];
        var volume = data.args.Length > 1 ? float.Parse(data.args[1]) : 1f;
        entity.audioManager.PlaySound(path, volume);
    }
    
    [AnimationEventHandler("std/AddActiveFrame")]
    public virtual void OnAddActiveFrame(AnimationEventData data) {

    }
    
    [AnimationEventHandler("std/PlaySoundLoop")]
    public virtual void OnPlaySoundLoop(AnimationEventData data) {
        var path = data.args[0];
        var volume = data.args.Length > 1 ? float.Parse(data.args[1]) : 1f;
        entity.audioManager.PlaySoundLoop(path, volume);
    }
    
    [AnimationEventHandler("std/StopSoundLoop")]
    public virtual void OnStopSoundLoop(AnimationEventData data) {
        entity.audioManager.StopSoundLoop(data.args[0]);
    }
}

public abstract class CharacterState : EntityState {
    public PlayerCharacter player { get; private set; }
    protected PlayerCharacter opponent => player.opponent;
    public abstract float inputPriority { get; }

    protected bool RevalidateInput() {
        return IsInputValid(GetCurrentInputBuffer());
    }
    
    protected InputBuffer GetCurrentInputBuffer() {
        if (!(entity is PlayerCharacter player))
            throw new NotSupportedException();

        return player.inputProvider.inputBuffer;
    }
    public CharacterState(Entity entity) : base(entity) {
        player = (PlayerCharacter)entity;
    }
    public abstract bool IsInputValid(InputBuffer buffer);
}

}
