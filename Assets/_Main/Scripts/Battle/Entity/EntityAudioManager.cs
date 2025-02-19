using System;
using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Scripts.Audio; 
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class EntityAudioManager : MonoBehaviour {
    private Entity entity;
    private AudioSource audioSource;

    private Dictionary<int, AudioHandle> loopSources { get; } = new();
    private int idCounter = 1;
    private Transform container;

    private void Start() {
        entity = GetComponent<Entity>();
        audioSource = GetComponent<AudioSource>();
        container = new GameObject("AudioContainer").transform;
        container.parent = transform;
    }

    public void PlaySound(string soundName, float volume = 1f, float pitch = 1f) {
        if (soundName == null) return;
        AssetManager.Get<AudioClip>(soundName, clip => {
            var go = new GameObject($"Player: {soundName}");
            go.transform.parent = container;
            var comp = go.AddComponent<AudioClipDataPlayer>();
            comp.Play(clip, volume, pitch);
        }); 
    }

    public void PlaySoundClip(string soundName) {
        if (soundName == null) return;
        AssetManager.Get<AudioClipData>(soundName, data => {
            var go = new GameObject($"Player: {soundName}");
            go.transform.parent = container;
            var comp = go.AddComponent<AudioClipDataPlayer>();
            comp.Play(data);
        });
    }
    
    public int PlaySoundLoop(string soundName, float volume = 1f, bool releaseOnStateEnd = false) {
        var id = idCounter++;
        AssetManager.Get<AudioClip>(soundName, clip => {
            var go = new GameObject($"LoopSource_{id}");
            go.transform.parent = transform.parent;
            
            var source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.volume = volume;
            source.Play();
            loopSources[id] = new AudioHandle(soundName, source, releaseOnStateEnd ? entity.activeState : null);
        });
        return id;
    }

    public void StopSoundLoop(int id, string tailSound = null, float tailVolume = 1f) {
        if (loopSources.ContainsKey(id)) {
            loopSources[id].Release();
            loopSources.Remove(id);
            
            if (tailSound != null) {
                PlaySound(tailSound, tailVolume);
            }
        }
    }

    
    public void StopSoundLoop(string soundName) {
        foreach (var id in loopSources.Keys.ToList()) {
            if (loopSources[id].audioId == soundName) {
                StopSoundLoop(id);
            }
        }
    }

    public class AudioHandle {
        public readonly string audioId;
        public readonly AudioSource source;
        public readonly EntityState state;
        public AudioHandle(string audioId, AudioSource source, EntityState state) {
            this.audioId = audioId;
            this.source = source;
            this.state = state;

            if (state != null) {
                state.onStateEnd.AddListener(OnStateEnd);   
            }
        }

        public void Release() {
            if (!source) return;
            if (state != null) state.onStateEnd.RemoveListener(OnStateEnd);
            Destroy(source.gameObject);
        }

        private void OnStateEnd() {
            Release();
        }
    }
}
}
