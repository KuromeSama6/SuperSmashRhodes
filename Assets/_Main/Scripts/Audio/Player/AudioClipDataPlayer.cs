using System;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Scripts.Audio {
public class AudioClipDataPlayer : MonoBehaviour {
    public AudioSource audioSource { get; private set; }
    public AudioClipData data { get; private set; }
    public bool loop { get; set; }
    public bool isPlaying => audioSource.isPlaying;

    private void Awake() {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start() {
        audioSource.playOnAwake = true;
    }

    public void Play(AudioClipData clip, bool loop = false) {
        data = clip;
        audioSource.clip = clip.clips.RandomChoice();
        this.loop = loop;
        audioSource.time = 0;
        audioSource.Play();
    }
    
    public void Play(AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = false) {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.time = 0;
        this.loop = loop;
        audioSource.Play();
        // Debug.Log(audioSource.pitch);
    }

    public void Load(Handle handle) {
        audioSource.volume = handle.volume;
        audioSource.pitch = handle.pitch;
        audioSource.clip = handle.clip;
        
        if (handle.data != null) {
            Play(handle.data, handle.loop);
        } else {
            Play(handle.clip, handle.volume, handle.pitch, handle.loop);
        }
        audioSource.time = handle.time;
    }

    private void Update() {
        if (data != null && audioSource) {
            audioSource.volume = data.volumeCurve.Evaluate(audioSource.time / audioSource.clip.length);
            audioSource.pitch = data.pitchCurve.Evaluate(audioSource.time / audioSource.clip.length);
        }

        if (!audioSource.isPlaying) {
            if (loop) {
                audioSource.Play();
            } else {
                data = null;
                Destroy(gameObject);
            }
        }
    }

    public Handle Serialize() {
        return new Handle(this);
    }
    
    public struct Handle {
        public readonly AudioClip clip;
        public readonly AudioClipData data;
        public readonly bool loop;
        public readonly float volume, pitch;
        public readonly float time;
        
        public Handle(AudioClipDataPlayer parent) {
            clip = parent.audioSource.clip;
            data = parent.data;
            loop = parent.loop;
            volume = parent.audioSource.volume;
            pitch = parent.audioSource.pitch;
            time = parent.audioSource.time;
        }
    }
}
}
