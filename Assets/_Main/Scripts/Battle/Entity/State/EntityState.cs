using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Spine;
using SuperSmashRhodes.Battle.Animation;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Runtime.Tokens;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using UnityEngine;
using UnityEngine.Events;

namespace SuperSmashRhodes.Battle.State {
public abstract class EntityState : NamedToken, IStateSerializable, IHandleSerializable {
    public Entity entity { get; protected set; }
    public EntityStateData stateData { get; private set; }
    public bool active { get; private set; }
    public int frame { get; private set; }
    public abstract EntityStateType type { get; }
    public virtual bool mayEnterState => true;
    public virtual AttackType invincibility => AttackType.NONE;
    public virtual bool isSelfCancellable => false;
    public virtual bool enableHitboxes => true;
    public int activeRoutines => routines.Count;

    public UnityEvent onStateEnd { get; } = new();
    public virtual CharacterStateFlag globalFlags => CharacterStateFlag.NONE;
    protected virtual EntityStateSerializationFlags serializationFlags => EntityStateSerializationFlags.NONE;
    protected virtual SubroutineFlags mainRoutineFlags => SubroutineFlags.NONE;
    
    public int interruptFrames { get; private set; }
    private int scheduledPauseAnimationFrames;
    [SerializationOptions(SerializationOption.EXCLUDE)]
    private readonly Dictionary<string, List<MethodInfo>> animationEventHandlers = new();
    [SerializationOptions(SerializationOption.EXCLUDE)]
    public List<StateSubroutine> routines { get; } = new();
    private StateSubroutine currentRoutine => routines.Count > 0 ? routines[0] : null;
    [SerializationOptions(SerializationOption.EXCLUDE)]
    private ReflectionSerializer reflectionSerializer;
    
    public EntityState(Entity entity) {
        this.entity = entity;
        reflectionSerializer = new(this);
        
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

        stateData = entity.CreateStateData(this);
    }

    private void Init() {
        OnStateBegin();
        routines.Clear();
        var routine = new StateSubroutine(() => MainRoutine(), 0, mainRoutineFlags);
        routines.Insert(0, routine);
        routine.Hydrate();

    // Debug.Log("push subroutine");
    }

    public void BeginState() {
        stateData = entity.CreateStateData(this);
        
        frame = 0;
        interruptFrames = 0;
        scheduledPauseAnimationFrames = 0;
        
        Init();
        active = true;
    }
    
    public void TickState() {
        if (!active) return;
        
        ++frame;

        // if (entity.entityId == 0) Debug.Log($"frame {frame} {currentRoutine.enumerator}"); 
        OnTick();
        
        // scheduled animation
        if (entity.shouldTickAnimation && !currentRoutine.flags.HasFlag(SubroutineFlags.PAUSE_ANIMATION)) {
            if (scheduledPauseAnimationFrames > 0) {
                --scheduledPauseAnimationFrames;
            
            } else {
                if (entity.animation) entity.animation.Tick();
            }   
        }
        
        // interrupt frames
        if (!entity.shouldTickState) return;
        
        if (interruptFrames > 0) {
            --interruptFrames;
            // Debug.Log("interrupt");
            return;
        }
        
        if (currentRoutine.enumerator.MoveNext()) {
            var current = currentRoutine.enumerator.Current;
            ++currentRoutine.timesTicked;
            HandleRoutineReturn(current);
            // if (entity.entityId == 0) Debug.Log($"routine tick, state={id} routine={currentRoutine}, rem={routines.Count}, current={current}");
            
        } else {
            if (routines.Count > 1) {
                var popped = routines[0];
                routines.RemoveAt(0);
                HandleRoutineReturn(null);
                frame = popped.parentFrame;
                if (entity.shouldTickAnimation) {
                    entity.animation.Tick();
                }
                // Debug.Log($"routine end, rem={routines.Count}");

            } else {
                EndState(entity.GetDefaultState());
            }
        }
    }

    public void EndState(EntityState nextState) {
        active = false;
        OnStateEnd(nextState);
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
        // if (entity.entityId == 0) Debug.Log($"handle ret {obj} {obj.GetType()}");
        if (obj is int framesSkipped) {
            interruptFrames += Mathf.Max(framesSkipped - 1, 0);
            return;
        }
        
        if (obj is float framesSkippedFloat) {
            interruptFrames += Mathf.Max((int)framesSkippedFloat - 1, 0);
            return;
        }

        if (obj is EntityStateYieldInstruction yieldInstruction) {
            yieldInstruction.Execute(stateData);
            return;
        }

        if (obj is IEnumerator enumerator) {
            routines.Insert(0, new StateSubroutine(() => enumerator, frame));
            return;
        }
        
        if (obj is StateSubroutine wrapper) {
            routines.Insert(0, wrapper);
            return;
        }
        
    }
    
    public void BeginSubroutine(IEnumerator routine, SubroutineFlags flags = SubroutineFlags.PAUSE_ANIMATION) {
        var wrapper = new StateSubroutine(() => routine, frame, flags);
        routines.Insert(0, wrapper);
        wrapper.Hydrate();
    }
    
    // Member methods
    protected void AddCancelOption(string stateName) {
        if (!entity.states.TryGetValue(stateName, out var state))
            throw new KeyNotFoundException($"State {stateName} not found");
            
        if (stateData.cancelOptions.Contains(state)) return;
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
        
        EndState(state);
        entity.BeginState(state);
    }
    
    protected void SetScheduledPauseAnimationFrames(int frames) {
        scheduledPauseAnimationFrames = frames;
    }
    
    // Virtual methods / Events
    protected virtual void OnTick() {
    }
    
    protected virtual void OnStateBegin() { }
    protected virtual void OnStateEnd(EntityState nextState) {
        entity.onStateEnd.Invoke(this);
    }
    public virtual void OnLand(LandingRecoveryFlag flag, int recoveryFrames) {}
    // Abstract methods
    public abstract IEnumerator MainRoutine();
    
    // Handlers
    
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
        entity.audioManager.PlaySoundLoop(path, volume, true);
    }
    
    [AnimationEventHandler("std/StopSoundLoop")]
    public virtual void OnStopSoundLoop(AnimationEventData data) {
        entity.audioManager.StopSound(data.args[0]);
    }
    
    [AnimationEventHandler("std/ApplyForce")]
    public virtual void OnApplyVelocity(AnimationEventData data) {
        // Debug.Log("apply force");
        var x = float.Parse(data.GetArg(0));
        var y = float.Parse(data.GetArg(1));
        var xCarried = float.Parse(data.GetArg(2, "0"));
        var yCarried = float.Parse(data.GetArg(3, "0"));
        

        var vel = new Vector2(x, y);
        var carried = new Vector2(xCarried, yCarried);

        entity.rb.linearVelocity *= carried;
        // Debug.Log(vel * new Vector2(entity.side == EntitySide.LEFT ? 1 : -1, 1));
        entity.rb.AddForce(vel * new Vector2(entity.side == EntitySide.LEFT ? 1 : -1, 1), ForceMode2D.Impulse);
    }
    
    public virtual void Serialize(StateSerializer serializer) {
        reflectionSerializer.Serialize(serializer);
        
        // routines
        var handles = routines.Select(c => c.GetHandle()).ToList();
        // Debug.Log($"routine handles {string.Join(", ", handles)}");
        serializer.PutList("routines", handles);
    }
    
    public virtual void Deserialize(StateSerializer serializer) {
        // Debug.Log($"begin deserialize {entity.entityId} {this} {frame} {entity.GetHashCode()}");
        // Debug.Log($"start, {routines.Count}, {currentRoutine}, int={interruptFrames}, vel={entity.rb.linearVelocity}");
        OnStateBegin();
        {
            // routines first
            var list = new List<IHandle>();
            serializer.GetList("routines", list);
            
            routines.Clear();
            foreach (var handle in list) {
                routines.Add((StateSubroutine)handle.Resolve());
            }
            
        }
        // Debug.Log($"reflections {entity.entityId}");
        reflectionSerializer.Deserialize(serializer);
        
        // Debug.Log($"after, {routines.Count}, {currentRoutine}, int={interruptFrames}, vel={entity.rb.linearVelocity}");
    }
    public virtual IHandle GetHandle() {
        return new EntityStateHandle(entity, id);
    }
}

public abstract class CharacterState : EntityState {
    public PlayerCharacter player { get; private set; }
    public int chargeLevel { get; protected set; }
    
    protected PlayerCharacter opponent => player.opponent;
    public abstract float inputPriority { get; }
    public virtual StateIndicatorFlag stateIndicator => StateIndicatorFlag.NONE;
    public virtual Hitstate hitstate => Hitstate.NONE;
    public bool charging { get; protected set; }
    public bool driveRelease => player.burst.driveRelease;
    
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

    protected override void OnStateBegin() {
        base.OnStateBegin();
        chargeLevel = 0;
        charging = false;
    }

    protected override void OnTick() {
        base.OnTick();
        {
            // charge
            if (this is IChargable chargable && !charging) {
                // Debug.Log($"chargable, entry = {chargable.chargeEntryFrame}, mayCharge = {chargable.mayCharge}");
                if (frame == chargable.chargeEntryFrame && chargable.mayCharge) {
                    // Debug.Log("charge start");
                    BeginSubroutine(chargable.ChargeRoutine());
                    OnChargeBegin();
                }
            }
        }
    }

    public void AddCharge(int levels) {
        chargeLevel += levels;
        OnCharge(chargeLevel);
    }

    protected virtual void OnChargeBegin() {
        charging = true;
    }
    protected virtual void OnCharge(int newLevel) {}
    public virtual InboundHitModifier OnHitByOther(AttackData attackData) {
        return InboundHitModifier.NONE;
    }
    
    public abstract bool IsInputValid(InputBuffer buffer);
    
    [AnimationEventHandler("std/ApplyNeutralPose")]
    public virtual void ApplyNeutralPose(AnimationEventData data) {
        player.animation.ApplyNeutralPose();
    }
}

}
