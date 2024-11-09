using System;
using System.Collections.Generic;
using NUnit.Framework;
using SingularityGroup.HotReload;
using Sirenix.OdinInspector;
using Spine.Unity;
using SuperSmashRhodes.Battle.Animation;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.State;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
/// <summary>
/// An entity is the most basic form of something that lives and moves, and runs a Spine animation.
/// Entities include player characters, projectiles, summons, etc. Particles are not entities.
/// </summary>
public abstract class Entity : MonoBehaviour {
    [Title("References")]
    public EntityConfiguration config;
    
    public EntityFacing facing { get; protected set; } = EntityFacing.LEFT;
    public EntityAnimationController animation { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityState activeState { get; private set; }

    public Dictionary<string, EntityState> states { get; } = new();

    // Entity Stats
    public float health { get; set; }
    
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
        
    }

    protected virtual void FixedUpdate() {
        // state
        {
            EnsureState();
            activeState.TickState();
            if (!activeState.active) activeState = null;
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

    public void BeginState(EntityState state) {
        if (state == null)
            throw new Exception("Cannot begin null state");
        
        if (activeState != null && activeState.active)
            activeState.EndState();

        activeState = state;
        state.BeginState();
    }
    
    // Implemented methods
    public virtual void RoundInit() {
        health = config.health; 
    }
    
    // Abstract Methods
    protected abstract EntityState GetDefaultState();

}
}
