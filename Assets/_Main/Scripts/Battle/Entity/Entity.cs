using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using NUnit.Framework;
using SingularityGroup.HotReload;
using Sirenix.OdinInspector;
using Spine.Unity;
using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Battle.Animation;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Runtime.Tokens;
using SuperSmashRhodes.Util;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace SuperSmashRhodes.Battle {
/// <summary>
/// An entity is the most basic form of something that lives and moves, and runs a Spine animation.
/// Entities include player characters, projectiles, summons, etc. Particles are not entities.
/// </summary>
public abstract class Entity : MonoBehaviour, IManualUpdate, IStateSerializable, IHandleSerializable {
    [Title("Configuration")]
    public string assetPath;
    
    [Title("References")]
    public Transform rotationContainer;
    public Transform socketsContainer;
    public EntityConfiguration config;
    // public List<EntityAssetLibrary> assetLibraries = new();

    public int entityId { get; private set; }
    public EntitySide side { get; set; } = EntitySide.LEFT;
    [SerializationOptions(SerializationOption.EXCLUDE)]
    public EntityAnimationController animation { get; private set; }
    public Rigidbody2D rb { get; private set; }
    [SerializationOptions(SerializationOption.EXPAND, -1)]
    public EntityState activeState { get; private set; }
    public EntityBoundingBoxManager boundingBoxManager { get; private set; }
    [SerializationOptions(SerializationOption.EXCLUDE)]
    public Dictionary<string, EntityState> states { get; } = new();
    [SerializationOptions(SerializationOption.EXCLUDE)]
    public EntityAudioManager audioManager { get; private set; }

    // Entity Stats
    public float health { get; set; }
    public float healthPercent => health / config.health;

    public bool logicStarted { get; private set; }
    // public EntityAssetLibrary assetLibrary { get; private set; }
    public PlayerCharacter owner { get; protected set; }
    public EntityState lastState { get; protected set; }
    public bool attached => transform.parent;
    public int slowdownFrames { get; set; }
    public UnityEvent<EntityState> onStateEnd { get; } = new();
    
    [SerializationOptions(SerializationOption.EXCLUDE)]
    public List<Entity> summons { get; } = new();
    
    private readonly List<AttackData> queuedInboundAttacks = new();
    
    private readonly Dictionary<string, CarriedStateVariable> carriedStateVariables = new();
    
    [NonSerialized]
    private ReflectionSerializer reflectionSerializer;
    [SerializationOptions(SerializationOption.EXCLUDE)]
    public bool initialized { get; private set; }

    [SerializationOptions(SerializationOption.EXCLUDE)]
    protected bool _serializedAttached { get; private set; } = true;

    private void Awake() {
        // Debug.Log($"awake {this}");
        animation = GetComponent<EntityAnimationController>();
        rb = GetComponent<Rigidbody2D>();
        boundingBoxManager = GetComponentInChildren<EntityBoundingBoxManager>();
        audioManager = GetComponent<EntityAudioManager>();
        reflectionSerializer = new(this);
        entityId = GameManager.inst.RegisterEntity(this).entityId;
        
        foreach (var stateLibrary in config.stateLibraries) {
            foreach (var name in stateLibrary.states) {
                string prefix = stateLibrary.useTokenNameAsPrefix ? (config.tokenName + "_") : stateLibrary.prefix;
                var tokenName = prefix + name;
                if (!EntityStateRegistry.inst.CreateInstance(tokenName, out var state, this)) {
                    Debug.LogWarning($"State {tokenName} not found");
                    continue;
                }

                states[tokenName] = state;
            }
        }
        
        // Debug.Log($"tokens, {string.Join(", ", states.Keys)}");
    }

    protected virtual void Start() {
        if (!initialized) Init();
    }

    public virtual void Init() {
        if (initialized)
            throw new Exception($"Entity(id {entityId}) already initialized!");
        initialized = true;
        // Debug.Log($"init {this}");
        
        GameStateManager.inst.RefreshComponentReferences();
    }
    
    public virtual void ManualUpdate() {
        
    }

    public virtual void LogicPreUpdate() {
    }

    public virtual void LogicUpdate() {
        if (!logicStarted) return;

        rb.simulated = shouldSimulatePhysics;
        
        if (TimeManager.inst.globalFreezeFrames > 0) {
            return;
        }

        if (slowdownFrames > 0) {
            --slowdownFrames;
            if (slowdownFrames % 2 == 0) {
                return;
            }
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

            if (activeState != null) {
                activeState.TickState();
                if (!activeState.active) {
                    activeState = null;
                    EnsureState();
                }   
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

        if (this is PlayerCharacter player && player.stateFlags.HasFlag(CharacterStateFlag.NO_NEW_STATE)) return;
        
        if (activeState != null && activeState.active) {
            lastState = activeState;
            activeState.EndState(state);
        }
        activeState = state;
        state.BeginState();
    }
    
    public EntityStateData CreateStateData(EntityState state) {
        var ret = new EntityStateData(this);
        foreach (var variable in carriedStateVariables.Values.ToArray()) {
            if (variable.targetState == null || Regex.IsMatch(state.id, variable.targetState)) {
                ret.carriedVariables[variable.key] = variable.value;
                carriedStateVariables.Remove(variable.key);
            }
        }
        
        return ret;
    }

    public virtual void BeginLogic() {
        logicStarted = true;
        if (animation) {
            animation.animation.timeScale = 1f;   
        }
    }

    public virtual void HandleEntityInteraction(IEntityBoundingBox from, IEntityBoundingBox to, EntityBBInteractionData data) {
        if (from.owningPlayer == to.owningPlayer) return;

        ulong fromType = (ulong)from.type;
        ulong toType = (ulong)to.type;
        if (BitUtil.CheckFlag(fromType, (ulong)BoundingBoxType.HURTBOX) && BitUtil.CheckFlag(toType, (ulong)BoundingBoxType.HURTBOX)) {
            return;
        }

        // TODO Clash Counters

        // Debug.Log($"from {from.name} {fromType} {from.owner} to {to.name} {toType} {to.owner}");

        // Hit
        if (BitUtil.CheckFlag(fromType, (ulong)BoundingBoxType.HITBOX) && BitUtil.CheckFlag(toType, (ulong)BoundingBoxType.HURTBOX)) {
            var ret = OnOutboundHit(to.owningPlayer, data);
            if (ret != null) {
                // to.owner.OnInboundHit(this, data);
                to.owningPlayer.QueueInboundAttack(new() {
                    attack = ret,
                    from = this,
                    to = to.owningPlayer,
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
        GameManager.inst.UnregisterEntity(entity);
        summons.Remove(entity);
        // Debug.Log("dest");
        Destroy(entity.gameObject);
    }

    public T GetSummon<T>() where T: Entity {
        return summons.Find(c => c is T) as T;
    }
    
    public void Attach(string socket) {
        var go = owner.socketsContainer.Find(socket);
        if (!go) {
            Debug.LogError($"Could not find socket [{socket}] on entity [{name}]");
            return;
        }

        transform.parent = go;
        rb.simulated = false;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }

    public Transform GetHopBoneFollower(string boneName) {
        var objectName = $"Bone_Hop_{boneName}";
        var go = owner.socketsContainer.Find(objectName);
        if (!go) {
            // create object
            go = new GameObject(objectName).transform;
            go.parent = owner.socketsContainer;
            go.localPosition = Vector3.zero;
            
            // bone follower
            var follower = go.gameObject.AddComponent<BoneFollower>();
            follower.SkeletonRenderer = owner.animation.animation;
            var res = follower.SetBone(boneName);

            if (!res) {
                Debug.LogError($"Could not find Spine bone [{boneName}] on entity [{name}]");
                Destroy(go.gameObject);
                return null;
            }
        }
        
        return go;
    }
    
    public void AttachToBone(string boneName) {
        var go = GetHopBoneFollower(boneName);
        
        transform.parent = go;
        rb.simulated = false;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }
    
    public void Detach() {
        transform.parent = null;
        rb.simulated = true;
    }

    /// <summary>
    /// Sets a state variable that is carried over to the next state. The target state may be specified by the targetState parameter as a regular expression that matches a state's id, or <code>null</code> to apply to all states.
    /// When the target state is next activated, this value will be present by the specified key on the state's stateData. After the state is activated, the value will be removed.
    /// </summary>
    /// <param name="key">The key to identify the value. Must be unique globally.</param>
    /// <param name="targetState">The target state.</param>
    /// <param name="value">The value.</param>
    public void SetCarriedStateVariable(string key, [CanBeNull] string targetState, object value) {
        var variable = new CarriedStateVariable(key, targetState, value);
        carriedStateVariables[key] = variable;
    }

    public void ApplyForwardVelocity(Vector2 force) {
        rb.AddForce(TranslateDirectionalForce(force), ForceMode2D.Impulse);
    }
    
// Implemented methods
    public virtual bool shouldTickAnimation => true;
    public virtual bool shouldTickState => true;
    public virtual bool shouldSimulatePhysics {
        get {
            if (transform.parent != null) return false;
            if (TimeManager.inst.globalFreezeFrames > 0) return false;
            if (slowdownFrames > 0 && slowdownFrames % 2 == 0) return false;
            return true;
        }
    }
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
    public abstract EntityState GetDefaultState();

    public virtual void Serialize(StateSerializer serializer) {
        reflectionSerializer.Serialize(serializer);

        {
            // position and velocity
            serializer.Put("transform/position", transform.position);
            serializer.Put("transform/rotation", transform.eulerAngles);
            serializer.Put("transform/scale", transform.localScale);
            
            serializer.Put("rb/velocity", rb.linearVelocity);
            serializer.Put("rb/angularVelocity", rb.angularVelocity);
            serializer.Put("rb/inertia", rb.inertia);
            serializer.Put("rb/drag", rb.linearDamping);
            serializer.Put("rb/angularDrag", rb.angularDamping);
            serializer.Put("rb/simulated", rb.simulated);
            serializer.Put("rb/totalForce", rb.totalForce);
            serializer.Put("rb/totalTorque", rb.totalTorque);
        }

        {
            // etc fields
            serializer.Put("etc/_serializedAttached", attached);
        }
        
        {
            // components
            var components = new StateSerializer();
            foreach (var serializable in GetComponentsInChildren<IStateSerializable>()) {
                if (serializable == this) continue;
                
                var pth = new StateSerializer();
                serializable.Serialize(pth);
                
                components.Put(serializable.GetType().FullName, pth.objects);
            }
            
            serializer.Put("components", components.objects);
        }

        {
            // summons
            var summonsSerializer = new StateSerializer();
            foreach (var summon in summons) {
                var pth = new StateSerializer();
                pth.Put("handle", summon.GetHandle());
                if (summon) {
                    var data = new StateSerializer();
                    summon.Serialize(data);
                    pth.Put("data", data.objects);   
                }
                
                summonsSerializer.Put(summon.entityId.ToString(), pth.objects);
            }
            serializer.Put("summons", summonsSerializer.objects);
        }
    }
    public virtual void Deserialize(StateSerializer serializer) {
        // Debug.Log("deser");
        // Debug.Log(boundingBoxManager);

        {
            // position and velocity
            transform.position = serializer.Get<Vector3>("transform/position");
            transform.eulerAngles = serializer.Get<Vector3>("transform/rotation");
            transform.localScale = serializer.Get<Vector3>("transform/scale");
            
            rb.linearVelocity = serializer.Get<Vector2>("rb/velocity");
            rb.angularVelocity = serializer.Get<float>("rb/angularVelocity");
            rb.inertia = serializer.Get<float>("rb/inertia");
            rb.linearDamping = serializer.Get<float>("rb/drag");
            rb.angularDamping = serializer.Get<float>("rb/angularDrag");
            rb.simulated = serializer.Get<bool>("rb/simulated");
            rb.totalForce = serializer.Get<Vector2>("rb/totalForce");
            rb.totalTorque = serializer.Get<float>("rb/totalTorque");
            
            // Debug.Log($"eid {entityId} vel: {rb.linearVelocity}");
            // this.CallLaterCoroutine(0.01f, () => {
            //     Debug.Log($"eid {entityId} vel2: {rb.linearVelocity}");
            // });
        }

        {
            // explicit fields
            _serializedAttached = serializer.Get<bool>("etc/_serializedAttached");
        }

        // if (activeState == null) EnsureState();
        reflectionSerializer.Deserialize(serializer);
        
        {
            // summons
            var summonsSerialized = serializer.GetObject("summons");
            summons.Clear();
            foreach (var (k, v) in summonsSerialized.objects) {
                var serialized = new StateSerializer((SerializedDictionary)v);
                var entity = (Entity)serialized.Get<IHandle>("handle").Resolve();
                // Debug.Log(entity.GetHandle());
                if (entity) {
                    entity.Deserialize(serialized.GetObject("data"));
                    if (!entity.initialized) entity.Init();
                    summons.Add(entity);
                }
            }
            
        }
        
        {
            // components
            var serialized = serializer.Get<SerializedDictionary>("components");
            var components = GetComponentsInChildren<IStateSerializable>().ToDictionary(c => c.GetType().FullName, c => c);
            
            foreach (var (typeName, data) in serialized) {
                if (!components.ContainsKey(typeName)) {
                    // Debug.LogWarning($"Component {typeName} not found");
                    continue;
                }

                var ser = new StateSerializer((SerializedDictionary)data);
                components[typeName].Deserialize(ser);
            }
        }
        
        // Debug.Log($"finish deser {GetHandle()}, {activeState}");
    }
    
    public virtual IHandle GetHandle() {
        return new EntityHandle(this);
    }
}

class CarriedStateVariable {
    public string key;
    public string targetState;
    public object value;
    
    public CarriedStateVariable(string key, string targetState, object value) {
        this.key = key;
        this.targetState = targetState;
        this.value = value;
    }
    
}
}
