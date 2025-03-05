using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Serialization;

using UnityEngine;
using UnityEngine.Events;
using AnimationState = Spine.AnimationState;

namespace SuperSmashRhodes.Battle.Animation {
public class EntityAnimationController : MonoBehaviour, IStateSerializable {
    [SerializationOptions(SerializationOption.EXCLUDE)]
    public SkeletonAnimation animation { get; private set; }
    private AnimationState state => animation.state;
    private float currentBlendProgress = 0f;
    [SerializationOptions(SerializationOption.EXCLUDE)]
    private PlayerCharacter player;
    private float currentTimeScale = 1f;
    private float extractedFrameTime = 0f;
    
    public float targetFrameRate => player.logicStarted ? player.activeState.stateData.targetFrameRate : 60f;

    [SerializationOptions(SerializationOption.EXCLUDE)]
    private ReflectionSerializer reflectionSerializer;
    [SerializationOptions(SerializationOption.EXCLUDE)]
    public bool disableEvents { get; set; }

    private void Awake() {
        reflectionSerializer = new(this);
    }

    private void Start() {
        animation = GetComponentInChildren<SkeletonAnimation>();
        state.Event += OnUserDefinedEvent;
        player = GetComponentInParent<PlayerCharacter>();
    }

    private void Update() {
        // blend updates
    }

    public void ApplyNeutralPose(bool applySlots = false) {
        animation.skeleton.SetBonesToSetupPose();
        if (applySlots) animation.skeleton.SetSlotsToSetupPose();
    }
    
    public void AddUnmanagedAnimation(string name, bool loop, float transitionTime = 0f, float timeScale = 1) {
        if (player.activeState != null && player.activeState.stateData != null) {
            ApplyNeutralPose(player.activeState.stateData.shouldApplySlotNeutralPose);
        }
        var track = state.GetCurrent(0);
        
        // transitionTime = Mathf.Max(Time.fixedDeltaTime, transitionTime);
        
        if (transitionTime == 0) {
            // force the last frame to play
            // state.Update(track.TrackEnd - track.TrackTime);
            state.ClearTrack(0);
            
        } else if (track != null) {
            track.Loop = false;
        }
        
        try {
            state.AddAnimation(0, name, loop, transitionTime);
            currentTimeScale = 1;
            extractedFrameTime = 0;

        } catch (ArgumentException e) {
            Debug.LogWarning($"Animation {name} not found in {animation.skeletonDataAsset.name}");
        }
        
    }

    public void Tick(int frames = 1) {
        if (animation == null) return;

        if (1 / targetFrameRate <= Time.fixedDeltaTime) {
            animation.Update(frames * Time.fixedDeltaTime * currentTimeScale);  
            return;
        }
        
        var frameTime = 1f / targetFrameRate;

        bool shouldPlay = extractedFrameTime == 0;
        extractedFrameTime += Time.fixedDeltaTime;
        if (extractedFrameTime > frameTime) {
            extractedFrameTime = 0;
            shouldPlay = true;
        }
        
        if (shouldPlay) {
            animation.Update(frames * frameTime * currentTimeScale * .5f);   
        }
    }

    public void FastForward(int frames) {
        var time = frames * Time.fixedDeltaTime;
        var track = state.GetCurrent(0);

        track.TrackTime += time;
    }

    public void SetFrame(int frame) {
        state.GetCurrent(0).TrackTime = frame * Time.fixedDeltaTime;
        extractedFrameTime = 0;
    }
    
    private void OnUserDefinedEvent(TrackEntry trackEntry, Spine.Event e) {
        var name = e.Data.Name;
        // Debug.Log($"Animation event: {name} {e.String} ({state.GetCurrent(0).AnimationTime}) (F{GameStateManager.inst.frame}) (disabled: {disableEvents})");
        if (disableEvents) return;
        var args = (e.String ?? "").Split();
        var data = new AnimationEventData(e.String, e.Int, e.Float, e.Data.AudioPath);
        player.activeState.HandleAnimationEvent(name, data);
    }

    public void Serialize(StateSerializer serializer) {
        reflectionSerializer.Serialize(serializer);
        serializer.Put("animation/id", state.GetCurrent(0).Animation.Name);
        serializer.Put("animation/time", state.GetCurrent(0).AnimationTime);
        serializer.Put("animation/loop", state.GetCurrent(0).Loop);
    }
    public void Deserialize(StateSerializer serializer) {
        reflectionSerializer.Deserialize(serializer);

        disableEvents = true;
        var animationId = serializer.Get<string>("animation/id");
        var time = serializer.Get<float>("animation/time");
        var loop = serializer.Get<bool>("animation/loop");
        AddUnmanagedAnimation(animationId, loop);
        // Tick((int)(time / Time.fixedDeltaTime));
        // state.GetCurrent(0).TrackTime = 0;
        
        state.GetCurrent(0).TrackTime = time;
        animation.Update(0);
        disableEvents = false;
        // Debug.Log($"Deserialized animation {player.playerIndex} t={state.GetCurrent(0).AnimationTime} {GameStateManager.inst.frame}");
    }
}

public class TrackBlend {
    public readonly int layerIndex;
    public readonly float toAlpha,totalTime;
    public readonly UnityEvent onComplete = new();
    
    public bool complete => time >= totalTime;
    
    private float time;
    private SkeletonAnimation anim;
    private TrackEntry targetTrack => anim.state.GetCurrent(layerIndex);
    private float fromAlpha;
    
    public TrackBlend(SkeletonAnimation animation, int layerIndex, float toAlpha, float totalTime) {
        anim = animation;
        this.layerIndex = layerIndex;
        this.toAlpha = toAlpha;
        this.totalTime = totalTime;

        fromAlpha = targetTrack.Alpha;
    }

    public void Tick(float deltaTime) {
        if (complete) return;
        time += deltaTime;
        targetTrack.Alpha = Mathf.Lerp(fromAlpha, toAlpha, time / totalTime);

        if (complete) {
            onComplete.Invoke();
            onComplete.RemoveAllListeners();
        }
    }
}
}
