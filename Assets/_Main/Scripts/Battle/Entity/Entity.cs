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
    public FScriptObject mainScript;
    
    public EntityFacing facing { get; protected set; } = EntityFacing.LEFT;
    public SkeletonAnimation animation { get; private set; }
    public FScriptRuntime runtime { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityTask task { get; private set; }
    public Dictionary<string, FScriptObject> moves { get; } = new();
    public Dictionary<string, AnimationClip> managedAnimations { get; } = new();
    
    public bool isPhysicallyMoving => !Mathf.Approximately(rb.linearVelocityX, 0) || !Mathf.Approximately(rb.linearVelocityY, 0);

    // Air States
    public bool isPhysicallyGrounded { get; private set; } = true;
    public int forcedAirborneFrames { get; set; }
    private bool _isLogicallyGrounded = true;
    
    public bool isLogicallyGrounded {
        get {
            if (forcedAirborneFrames > 0) return false;
            return _isLogicallyGrounded;
        }
        set => _isLogicallyGrounded = value;
    }
    
    protected virtual void Start() {
        animation = GetComponent<SkeletonAnimation>();
        rb = GetComponent<Rigidbody2D>();
        runtime = GetComponent<FScriptRuntime>();
        
        // load animations
        // foreach (var reference in config.animationClipReferences) {
        //     foreach (var clip in reference.clips) {
        //         managedAnimations[clip.name] = clip;
        //     }
        // } 
    }

    protected virtual void Update() {
        
    }

    protected virtual void FixedUpdate() {
        if (managedXVelocityLimit >= 0) {
            rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -managedXVelocityLimit, managedXVelocityLimit);
        }
        
        UpdateGroundedState();
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
    

    private void UpdateGroundedState() {
        isPhysicallyGrounded = transform.position.y < 0.02f;
        if (!isLogicallyGrounded && isPhysicallyGrounded && forcedAirborneFrames == 0) {
            isLogicallyGrounded = true;
        }

        if (forcedAirborneFrames > 0) forcedAirborneFrames--;
    }
    
    /// <summary>
    /// The current limit on the entity's x velocity. If -1, there is no limit.
    /// Dynamically adjusted by the entity's implementation.
    /// </summary>
    public virtual float managedXVelocityLimit => -1;
}
}
