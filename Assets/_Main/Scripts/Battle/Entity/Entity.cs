using System;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using SingularityGroup.HotReload;
using Sirenix.OdinInspector;
using Spine.Unity;
using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Battle.Animation;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Util;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace SuperSmashRhodes.Battle {
/// <summary>
/// An entity is the most basic form of something that lives and moves, and runs a Spine animation.
/// Entities include player characters, projectiles, summons, etc. Particles are not entities.
/// </summary>
public abstract class Entity : MonoBehaviour {
    [Title("References")]
    public Transform rotationContainer;
    public Transform socketsContainer;
    public EntityConfiguration config;
    // public List<EntityAssetLibrary> assetLibraries = new();

    public EntitySide side { get; set; } = EntitySide.LEFT;
    public EntityAnimationController animation { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityState activeState { get; private set; }
    public EntityBoundingBoxManager boundingBoxManager { get; private set; }
    public Dictionary<string, EntityState> states { get; } = new();

    public EntityAudioManager audioManager { get; private set; }

    // Entity Stats
    public float health { get; set; }

    public bool logicStarted { get; private set; }
    // public EntityAssetLibrary assetLibrary { get; private set; }
    public PlayerCharacter owner { get; protected set; }
    
    private List<AttackData> queuedInboundAttacks = new();
    private List<Entity> summons { get; } = new();

    protected virtual void Start() {
        animation = GetComponent<EntityAnimationController>();
        rb = GetComponent<Rigidbody2D>();
        boundingBoxManager = GetComponentInChildren<EntityBoundingBoxManager>();
        audioManager = GetComponent<EntityAudioManager>();

        // load states
        foreach (var stateLibrary in config.stateLibraries) {
            foreach (var name in stateLibrary.states) {
                string prefix = stateLibrary.useTokenNameAsPrefix ? (config.tokenName + "_") : stateLibrary.prefix;
                var tokenName = prefix + name;
                if (!EntityStateRegistry.inst.CreateInstance(tokenName, out var state, this)) {
                    Debug.LogError($"State {tokenName} not found");
                    continue;
                }

                states[tokenName] = state;
            }
        }

        // merge asset libs
        // assetLibrary = ScriptableObject.CreateInstance<EntityAssetLibrary>();
        // foreach (var lib in assetLibraries) {
        //     assetLibrary.MergeFrom(lib);
        // }

        // Debug.Log($"Loaded states {string.Join(", ", states.Keys)}");
    }

    protected virtual void Update() { }

    protected virtual void FixedUpdate() {
        if (!logicStarted) return;

        if (TimeManager.inst.globalFreezeFrames > 0) {
            return;
        }

        // state
        {
            HandleInboundAttacks();
            EnsureState();

            if (this is PlayerCharacter player) {
                if (player.playerIndex == 0) {
                    // Debug.Log($"P0 {activeState.id}#{activeState.frame} P1 {player.opponent.activeState.id}#{player.opponent.activeState.frame} stun {player.opponent.frameData.hitstunFrames}");

                }
            }

            activeState.TickState();
            if (!activeState.active) {
                activeState = null;
                EnsureState();
            }
        }

        OnTick();
    }

    private void HandleInboundAttacks() {
        foreach (var attack in queuedInboundAttacks) {
            OnInboundHit(attack);
        }
        queuedInboundAttacks.Clear();
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
            var ret = OnOutboundHit(to.owner, data);
            if (ret != null) {
                // to.owner.OnInboundHit(this, data);
                to.owner.QueueInboundAttack(new() {
                    attack = ret,
                    from = this,
                    to = to.owner,
                    interactionData = data,
                    result = AttackResult.PENDING
                });
            }
        }

    }

    public void QueueInboundAttack(AttackData attack) {
        queuedInboundAttacks.Add(attack);
    }

    public Vector2 TranslateDirectionalForce(Vector2 force) {
        return new Vector2(force.x * (side == EntitySide.LEFT ? 1f : -1f), force.y);
    }

    public T Summon<T>(string path) where T: Entity {
        var prefab = AssetManager.Get<GameObject>(path);
        if (!prefab) {
            Debug.LogError($"Could not summon prefab at [{path}]. The path is either incorrect, or the prefab is not loaded. Summons require relevant prefabs to be preloaded.");
            return null;
        }

        var go = Instantiate(prefab);
        var entity = go.GetComponent<T>();
        if (!entity) {
            Debug.LogError($"The prefab at [{path}] does not have a component of type [{typeof(T)}].");
            Destroy(go);
            return null;
        }

        entity.owner = owner;
        entity.side = side;
        summons.Add(entity);
        
        entity.BeginLogic();
        
        return entity;
    }
    
    public void DestroySummon(Entity entity) {
        if (!summons.Contains(entity)) return;
        summons.Remove(entity);
        Destroy(entity.gameObject);
    }

// Implemented methods
    public virtual bool shouldTickAnimation => true;
    public virtual bool shouldTickState => true;
    public virtual void OnRoundInit() {
        health = config.health;
    }
    protected virtual void OnTick() {}
    protected virtual IAttack OnOutboundHit(Entity victim, EntityBBInteractionData data) {
        return null;
    }
    protected virtual void OnInboundHit(AttackData attack) {
        
    }
    
    // Abstract Methods
    protected abstract EntityState GetDefaultState();

}
}
