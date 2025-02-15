using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Events;
using AnimationState = Spine.AnimationState;

namespace SuperSmashRhodes.Battle.Animation {
public class EntityAnimationController : MonoBehaviour {
    public SkeletonAnimation animation { get; private set; }
    private AnimationState state => animation.state;
    private float currentBlendProgress = 0f;
    private List<TrackBlend> blends = new();
    private PlayerCharacter player;
    private float currentTimeScale = 1f;

    private float extractedFrameTime = 0f;
    
    public float targetFrameRate => player.activeState.stateData.targetFrameRate;
    
    private void Start() {
        animation = GetComponentInChildren<SkeletonAnimation>();
        state.Event += OnUserDefinedEvent;
        player = GetComponentInParent<PlayerCharacter>();
    }

    private void Update() {
        // blend updates
    }

    public void ApplyNeutralPose() {
        animation.skeleton.SetBonesToSetupPose();
    }
    
    public void AddUnmanagedAnimation(string name, bool loop, float transitionTime = 0f, float timeScale = 1) {
        ApplyNeutralPose();
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

    public void SetFrame(int frame) {
        state.GetCurrent(0).TrackTime = frame * Time.fixedDeltaTime;
        extractedFrameTime = 0;
    }
    
    private void OnUserDefinedEvent(TrackEntry trackEntry, Spine.Event e) {
        var name = e.Data.Name;
        var args = (e.String ?? "").Split();
        
        var data = new AnimationEventData(e.String, e.Int, e.Float, e.Data.AudioPath);
        player.activeState.HandleAnimationEvent(name, data);
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
