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
    
    public EntitySide side { get; protected set; } = EntitySide.LEFT;
    public EntityAnimationController animation { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityState activeState { get; private set; }
    public Dictionary<string, EntityState> states { get; } = new();
    // Entity Stats
    public float health { get; set; }

    private bool logicStarted;
    
    protected virtual void Start() {
        animation = GetComponent<EntityAnimationController>();
        rb = GetComponent<Rigidbody2D>();
        
        // load states
        foreach (var stateLibrary in config.stateLibraries) {
            foreach (var name in stateLibrary.states) {
                if (!EntityStateRegistry.inst.CreateInstance(name, out var state, this))
                   throw new Exception($"State {name} not found");
               
                states[name] = state;
            }
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
        
        // state
        {
            EnsureState();
            activeState.TickState();
            if (!activeState.active) {
                activeState = null;
                EnsureState(); 
            }
        }
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

    public void BeginLogic() {
        logicStarted = true;
    }

    public virtual void HandleEntityInteraction(EntityBoundingBox from, EntityBoundingBox to) {
        ulong fromType = (ulong)from.type;
        ulong toType = (ulong)to.type;
        
        // TODO Clash Counters
        
        // Hit
        if (BitUtil.CheckFlag(fromType, (ulong)BoundingBoxType.HITBOX) && BitUtil.CheckFlag(toType, (ulong)BoundingBoxType.HURTBOX)) {
            bool success = OnOutboundHit(to.owner);
            if (success) to.owner.OnInboundHit(this);
        }
        
    }
    
    // Implemented methods
    public virtual void OnRoundInit() {
        health = config.health;
    }
    protected virtual bool OnOutboundHit(Entity victim) {
        return false;
    }
    protected virtual void OnInboundHit(Entity attacker) {
        
    }
    
    // Abstract Methods
    protected abstract EntityState GetDefaultState();

}
}
