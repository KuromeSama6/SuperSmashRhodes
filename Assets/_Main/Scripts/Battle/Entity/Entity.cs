using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sirenix.OdinInspector;
using Spine.Unity;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.FScript;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
/// <summary>
/// An entity is the most basic form of something that lives and moves, and runs a Spine animation.
/// Entities include player characters, projectiles, summons, etc. Particles are not entities.
/// </summary>
public abstract class Entity : MonoBehaviour {
    [Title("References")]
    public EntityConfiguration config;
    public List<MoveSet> moveSets = new();
    
    public EntityFacing facing { get; protected set; } = EntityFacing.LEFT;
    public SkeletonMecanim skeleton { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityState state { get; private set; }
    public Dictionary<string, FScriptObject> moves { get; } = new();
    
    public bool isPhysicallyMoving => !Mathf.Approximately(rb.linearVelocityX, 0) || !Mathf.Approximately(rb.linearVelocityY, 0);
    public virtual bool isBusy {
        get {
            if (state != null) return true;
            return false;
        }
    }
    
    protected virtual void Start() {
        skeleton = GetComponent<SkeletonMecanim>();
        rb = GetComponent<Rigidbody2D>();
        
        // load moves
        foreach (var set in moveSets) {
            foreach (var move in set.moves) {
                var runtimeMove = move.DetachedCopy();
                runtimeMove.Load();
                moves[runtimeMove.descriptor.id] = runtimeMove;
            }
        }
    }

    protected virtual void Update() { }

    protected virtual void FixedUpdate() {
        if (managedXVelocityLimit >= 0) {
            rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -managedXVelocityLimit, managedXVelocityLimit);
        }
    }

    public void RunMove(string name) {
        if (!moves.ContainsKey(name))
            throw new ArgumentException($"{name}: move not found");
        
        RunMove(moves[name]);
    }
    
    public void RunMove(FScriptObject script) {
        state = new EntityState(this, script);
        StartCoroutine(state.Begin());

        state.onEnded.AddListener(OnStateEnded);
    }
    
    public void AddContinuousForce(Vector2 force) {
        rb.AddForce(force, ForceMode2D.Force);
    }
    
    public void AddSimulatedForce(Vector2 force) {
        transform.Translate(force * (Time.fixedDeltaTime));
    }

    public void AddCarriedForce(Vector2 force) {
        rb.AddForce(force, ForceMode2D.Impulse);
    }
    
    private void OnStateEnded(EntityState state) {
        this.state = null;
        state.Dispose();
    }
    
    /// <summary>
    /// The current limit on the entity's x velocity. If -1, there is no limit.
    /// Dynamically adjusted by the entity's implementation.
    /// </summary>
    public virtual float managedXVelocityLimit => -1;
}
}
