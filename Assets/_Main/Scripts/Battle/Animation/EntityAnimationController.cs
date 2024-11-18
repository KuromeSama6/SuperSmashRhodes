using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
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
    
    private void Start() {
        animation = GetComponentInChildren<SkeletonAnimation>();
        state.Event += OnUserDefinedEvent;
        player = GetComponentInParent<PlayerCharacter>();
    }

    private void Update() {
        // blend updates
    }

    public void AddUnmanagedAnimation(string name, bool loop, float transitionTime = 0f) {
        var track = state.GetCurrent(0);
        // transitionTime = Mathf.Max(Time.fixedDeltaTime, transitionTime);
        
        if (transitionTime == 0) {
            // force the last frame to play
            // state.Update(track.TrackEnd - track.TrackTime);
            state.ClearTrack(0);
            
        } else {
            track.Loop = false;
        }
        state.AddAnimation(0, name, loop, transitionTime);
    }

    public void Tick(int frames = 1) {
        if (animation == null) return;
        animation.Update(frames * Time.fixedDeltaTime);
        
    }
    
    private void OnUserDefinedEvent(TrackEntry trackEntry, Spine.Event e) {
        var name = e.Data.Name;
        var args = (e.String ?? "").Split();
        
        switch (name) {
            case "PlaySound":
                var path = args[0];
                var volume = args.Length > 1 ? float.Parse(args[1]) : 1f;
                player.audioManager.PlaySound(path, volume);
                break;
        }

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
