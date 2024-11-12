using System;
using System.Collections.Generic;
using NUnit.Framework;
using SingularityGroup.HotReload;
using Sirenix.OdinInspector;
using Spine.Unity;
using SuperSmashRhodes.Battle.Animation;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
/// <summary>
/// An entity is the most basic form of something that lives and moves, and runs a Spine animation.
/// Entities include player characters, projectiles, summons, etc. Particles are not entities.
/// </summary>
public abstract class Entity : MonoBehaviour {
    [Title("References")]
    public Transform rotationContainer;
    public EntityConfiguration config;
    public List<EntityAssetLibrary> assetLibraries = new();
    
    public EntitySide side { get; protected set; } = EntitySide.LEFT;
    public EntityAnimationController animation { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityState activeState { get; private set; }
    public EntityBoundingBoxManager boundingBoxManager { get; private set; }
    public Dictionary<string, EntityState> states { get; } = new();
    public EntityAudioManager audioManager { get; private set; }
    // Entity Stats
    public float health { get; set; }

    public bool logicStarted { get; private set; }
    public EntityAssetLibrary assetLibrary { get; private set; }
    
    protected virtual void Start() {
        animation = GetComponent<EntityAnimationController>();
        rb = GetComponent<Rigidbody2D>();
        boundingBoxManager = GetComponentInChildren<EntityBoundingBoxManager>();
        audioManager = GetComponent<EntityAudioManager>();
        
        // load states
        foreach (var stateLibrary in config.stateLibraries) {
            foreach (var name in stateLibrary.states) {
                if (!EntityStateRegistry.inst.CreateInstance(name, out var state, this))
                   throw new Exception($"State {name} not found");
               
                states[name] = state;
            }
        }
        
        // merge asset libs
        assetLibrary = ScriptableObject.CreateInstance<EntityAssetLibrary>();
        foreach (var lib in assetLibraries) {
            assetLibrary.MergeFrom(lib);
        }

        // Debug.Log($"Loaded states {string.Join(", ", states.Keys)}");
    }

    protected virtual void Update() {
        {
            // facing animation
            float target = side == EntitySide.LEFT ? 0 : 180;
            rotationContainer.eulerAngles = new Vector3(0, Mathf.Lerp(rotationContainer.eulerAngles.y, target, Time.deltaTime * 20f), 0);
        }
    }

    protected virtual void FixedUpdate() {
        if (!logicStarted) return;
        
        if (PhysicsTickManager.inst.globalFreezeFrames > 0) {
            return;
        }
        
        // state
        {
            EnsureState();
            activeState.TickState();
            if (!activeState.active) {
                activeState = null;
                EnsureState(); 
            }
        }

        OnTick();
    }

    public void EnsureState() {
        if (activeState == null) {
            var state = GetDefaultState();
            if (state == null)
                throw new Exception("No state assigned"); 
            BeginState(state);
        }
    }

    public void BeginState(string state) {
        BeginState(states[state]);
    }
    
    public void BeginState(EntityState state) {
        if (state == null)
            throw new Exception("Cannot begin null state");
        
        if (activeState != null && activeState.active)
            activeState.EndState();

        activeState = state;
        state.BeginState();
    }

    public virtual void BeginLogic() {
        logicStarted = true;
        
    }

    public virtual void HandleEntityInteraction(EntityBoundingBox from, EntityBoundingBox to, EntityBBInteractionData data) {
        if (from.owner == to.owner) return;
        
        ulong fromType = (ulong)from.type;
        ulong toType = (ulong)to.type;
        if (BitUtil.CheckFlag(fromType, (ulong)BoundingBoxType.HURTBOX) && BitUtil.CheckFlag(toType, (ulong)BoundingBoxType.HURTBOX)) {
            return;
        }
        
        // TODO Clash Counters
        
        // Debug.Log($"from {from.name} {fromType} {from.owner} to {to.name} {toType} {to.owner}");
        
        // Hit
        if (BitUtil.CheckFlag(fromType, (ulong)BoundingBoxType.HITBOX) && BitUtil.CheckFlag(toType, (ulong)BoundingBoxType.HURTBOX)) {
            bool success = OnOutboundHit(to.owner, data);
            if (success) to.owner.OnInboundHit(this, data);
        }
        
    }
    
    // Implemented methods
    public virtual void OnRoundInit() {
        health = config.health;
    }
    protected virtual void OnTick() {}
    protected virtual bool OnOutboundHit(Entity victim, EntityBBInteractionData data) {
        return false;
    }
    protected virtual void OnInboundHit(Entity attacker, EntityBBInteractionData data) {
        
    }
    
    // Abstract Methods
    protected abstract EntityState GetDefaultState();

}
}
