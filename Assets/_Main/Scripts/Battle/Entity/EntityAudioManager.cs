using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Scripts.Audio; 
using UnityEngine;
using Object = UnityEngine.Object;

namespace SuperSmashRhodes.Battle {
public class EntityAudioManager : MonoBehaviour, IStateSerializable, IManualUpdate {
    public Entity entity { get; private set; }
    public Dictionary<int, AudioHandle> handles { get; } = new();
    private int idCounter = 1;
    private Transform container;

    private void Start() {
        entity = GetComponent<Entity>();
        container = new GameObject("AudioContainer").transform;
        container.parent = transform;
        
        entity.onStateEnd.AddListener(OnStateEnd);
    }

    public int PlaySound(string soundName, float volume = 1f, float pitch = 1f) {
        if (soundName == null) return -1;
        var id = idCounter++;
        AssetManager.Get<AudioClip>(soundName, clip => {
            var go = new GameObject($"Player: {soundName}");
            go.transform.parent = container;
            var source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.Play();
            
            handles[id] = new AudioHandle(this, soundName, source, false);
        }); 
        
        return id;
    }

    public int PlaySoundClip(string soundName) {
        if (soundName == null) return -1;
        var id = idCounter++;
        AssetManager.Get<AudioClipData>(soundName, data => {
            var go = new GameObject($"Player: {soundName}");
            go.transform.parent = container;
            var comp = go.AddComponent<AudioClipDataPlayer>();
            comp.Play(data);
            
            handles[id] = new AudioHandle(this, soundName, comp, false);
        });
        
        return id;
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
            handles[id] = new AudioHandle(this, soundName, source, releaseOnStateEnd);
        });
        return id;
    }

    public void StopSound(int id, string tailSound = null, float tailVolume = 1f) {
        if (handles.ContainsKey(id)) {
            handles[id].Release();
            handles.Remove(id);
            
            if (tailSound != null) {
                PlaySound(tailSound, tailVolume);
            }
        }
    }

    
    public void StopSound(string soundName) {
        foreach (var id in handles.Keys.ToList()) {
            if (handles[id].audioId == soundName) {
                StopSound(id);
            }
        }
    }

    private void StopAllSounds() {
        foreach (var id in handles.Keys.ToList()) {
            StopSound(id);
        }
    }
    
    private void OnStateEnd(EntityState _) {
        foreach (var id in handles.Keys.ToList()) {
            if (handles[id].destroyOnStateEnd) {
                StopSound(id);
            }
        }
    }

    private void Prune() {
        foreach (var key in handles.Keys.ToList()) {
            if (!handles[key].valid) {
                handles.Remove(key);
            }
        }
    }
    
    public void Serialize(StateSerializer serializer) {
        Prune();
        serializer.PutDict("handles", handles.ToDictionary(c => c.Key, c => c.Value.GetHandle()));
    }
    
    public void Deserialize(StateSerializer serializer) {
        StopAllSounds();
        handles.Clear();
        var buf = new Dictionary<int, IHandle>();
        serializer.GetDict("handles", buf);
        foreach (var (key, value) in buf) {
            value.Resolve();
        }
    }

    public void ManualUpdate() {
        
    }
    public void EngineUpdate() {
        Prune();
    }
}

public class AudioHandle : IHandleSerializable {
    public readonly Entity entity;
    public readonly string audioId;
    public readonly AudioSource source;
    public readonly AudioClipDataPlayer player;
    public readonly bool destroyOnStateEnd;
    
    public bool valid => source || player;
    
    public AudioHandle(EntityAudioManager manager, string audioId, AudioSource source, bool destroyOnStateEnd) {
        entity = manager.entity;
        this.audioId = audioId;
        this.source = source;
        this.destroyOnStateEnd = destroyOnStateEnd;
        // Debug.Log("Created audio handle (normal)");
    }
    
    public AudioHandle(EntityAudioManager manager, string audioId, AudioClipDataPlayer player, bool destroyOnStateEnd) {
        entity = manager.entity;
        this.audioId = audioId;
        this.player = player;
        this.destroyOnStateEnd = destroyOnStateEnd;
        // Debug.Log("Created audio handle (clip)");
    }

    public void Release() {
        if (source) Object.Destroy(source.gameObject);
        if (player) Object.Destroy(player.gameObject);
    }

    public IHandle GetHandle() {
        return new Handle(this);
    }

    private struct Handle : IHandle {
        private IHandle entity;
        private string audioId;
        private bool destroyOnStateEnd;
        private bool isClip;
        private bool loop;
        private float volume;
        private float pitch;
        private float time;
        
        public Handle(AudioHandle handle) {
            entity = handle.entity.GetHandle();
            audioId = handle.audioId;
            destroyOnStateEnd = handle.destroyOnStateEnd;
            isClip = handle.player != null;
            loop = handle.source?.loop ?? handle.player.loop;
            volume = handle.source?.volume ?? 1;
            pitch = handle.source?.pitch ?? 1;
            time = handle.source?.time ?? handle.player.audioSource.time;
        }
        
        public object Resolve() {
            var manager = ((Entity)entity.Resolve()).audioManager;

            int ret;
            if (isClip) {
                ret = manager.PlaySoundClip(audioId);
                
            } else {
                if (loop) {
                    ret = manager.PlaySoundLoop(audioId, volume, destroyOnStateEnd);
                } else {
                    ret = manager.PlaySound(audioId, volume, pitch);
                }
            }

            var retHandle = manager.handles[ret];
            if (isClip) retHandle.player.audioSource.time = time;
            else retHandle.source.time = time;
            
            return manager.handles[ret];
        }
    }
}
}
