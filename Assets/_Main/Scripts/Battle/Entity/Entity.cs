using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine.Unity;
using SuperSmashRhodes.Battle.Enums;
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
    
    public bool isPhysicallyMoving => !Mathf.Approximately(rb.linearVelocityX, 0) || !Mathf.Approximately(rb.linearVelocityY, 0);
    
    protected virtual void Start() {
        skeleton = GetComponent<SkeletonMecanim>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update() { }

    protected virtual void FixedUpdate() {
        if (managedXVelocityLimit >= 0) {
            rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -managedXVelocityLimit, managedXVelocityLimit);
        }
    }
    
    public void AddContinuousForce(Vector2 force) {
        rb.AddForce(force, ForceMode2D.Force);
    }

    public void AddSimulatedForce(Vector2 force) {
        transform.Translate(force * (Time.fixedDeltaTime));
    }

    /// <summary>
    /// The current limit on the entity's x velocity. If -1, there is no limit.
    /// Dynamically adjusted by the entity's implementation.
    /// </summary>
    public virtual float managedXVelocityLimit => -1;
}
}
