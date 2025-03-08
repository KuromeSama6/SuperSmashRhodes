using System;
using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Scripts.Audio;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SuperSmashRhodes.Battle.Audio {
public class BattleAudioManager : SingletonBehaviour<BattleAudioManager>, IEngineUpdateListener, IAutoSerialize {
    private readonly Dictionary<int, AudioHandle> active = new();
    private readonly Dictionary<string, HashSet<int>> pathMap = new();
    private readonly HashSet<Entity> stateEndListeners = new();
    private int handleCounter = 0;

    private void Start() {
        
    }

    private void Update() {
        Prune();
    }

    private void Prune() {
        foreach (var (id, handle) in active.ToList()) {
            if (!handle.player) {
                active.Remove(id);
            }
        }
    }
    
    public int Play(Entity entity, AudioClip clip, bool loop = false, float volume = 1f, ActiveAudioReleaseMode releaseMode = ActiveAudioReleaseMode.MANUAL, float pitch = 1f) {
        if (!clip) return -1;
        var id = ++handleCounter;
        PlayInternal(id, entity, clip, loop, volume, releaseMode, pitch);
        return id;
    }

    public int Play(Entity entity, string path, bool loop = false, float volume = 1f, ActiveAudioReleaseMode releaseMode = ActiveAudioReleaseMode.MANUAL, float pitch = 1f) {
        if (string.IsNullOrEmpty(path)) return -1;
        var id = ++handleCounter;
        AssetManager.Get<Object>(path, res => {
            if (res is AudioClip clip) PlayInternal(id, entity, clip, loop, volume, releaseMode, pitch);
            else if (res is AudioClipData clipData) PlayInternal(id, entity, clipData, releaseMode, loop);
            else throw new ArgumentException($"{path} is not an AudioClip or AudioClipData");
        });

        if (!pathMap.ContainsKey(path)) pathMap[path] = new();
        pathMap[path].Add(id);
        return id;
    }
    
    private void PlayInternal(int id, Entity entity, AudioClip clip, bool loop = false, float volume = 1f, ActiveAudioReleaseMode releaseMode = ActiveAudioReleaseMode.ON_STATE_END, float pitch = 1f) {
        if (!stateEndListeners.Contains(entity)) {
            stateEndListeners.Add(entity);
            entity.onStateEnd.AddListener(state => OnStateEnd(entity, state));
        }

        var audio = CreatePlayer(id);
        audio.Play(clip, volume, pitch, loop);
        active.Add(id, new AudioHandle(entity, audio, releaseMode));
    }
    
    private AudioClipDataPlayer PlayInternal(int id, Entity entity, AudioClipData data, ActiveAudioReleaseMode releaseMode = ActiveAudioReleaseMode.ON_STATE_END, bool loop = false) {
        if (!stateEndListeners.Contains(entity)) {
            stateEndListeners.Add(entity);
            entity.onStateEnd.AddListener(state => OnStateEnd(entity, state));
        }

        var audio = CreatePlayer(id);
        audio.Play(data, loop);
        active.Add(id, new AudioHandle(entity, audio, releaseMode));
        
        return audio;
    }

    private AudioClipDataPlayer CreatePlayer(int id) {
        var go = new GameObject($"AudioHandle_Data_{id}");
        go.transform.parent = transform;
        AudioClipDataPlayer audio = go.AddComponent<AudioClipDataPlayer>();
        return audio;
    }
    
    public void ReleaseByPath(string path) {
        if (string.IsNullOrEmpty(path)) return;
        if (!pathMap.TryGetValue(path, out var ids)) return;
        foreach (var id in ids) {
            Release(id);
        }
        pathMap.Remove(path);
    }
    
    public void Release(int id) {
        if (!active.TryGetValue(id, out var handle)) return;
        ReleaseInternal(handle);
        active.Remove(id);
    }

    private void ReleaseInternal(AudioHandle handle) {
        if (!handle.player) return;
        Destroy(handle.player.gameObject);
    }

    private void ReleaseAll() {
        foreach (var (id, handle) in active.ToList()) {
            Release(id);
        }
        
        active.Clear();
        foreach (var set in pathMap.Values) {
            set.Clear();
        }
    }

    private void OnStateEnd(Entity entity, EntityState state) {
        foreach (var (id, handle) in active.ToList()) {
            if (handle.entity != entity) continue;
            if (handle.releaseMode == ActiveAudioReleaseMode.ON_STATE_END) {
                Release(id);
            }
        }
    }
    
    public void Serialize(StateSerializer serializer) {
        Prune();
        foreach (var (id, handle) in active) {
            StateSerializer pth = new();
            pth.Put("entity", handle.entity);
            pth.Put("data", handle.player.Serialize());
            pth.Put("releaseMode", handle.releaseMode);
            
            serializer.Put(id.ToString(), pth);
        }
    }
    
    public void Deserialize(StateSerializer serializer) {
        ReleaseAll();
        
        foreach (var (key, value) in serializer.objects) {
            if (!int.TryParse(key, out var id)) continue;
            var pth = new StateSerializer((SerializedDictionary)value);
            
            var entity = pth.GetHandle<Entity>("entity");
            var data = pth.Get<AudioClipDataPlayer.Handle>("data");
            var releaseMode = pth.Get<ActiveAudioReleaseMode>("releaseMode");
            
            var player = CreatePlayer(id);
            player.Load(data);
            active.Add(id, new AudioHandle(entity, player, releaseMode));
        }
    }
}

internal struct AudioHandle {
    public readonly Entity entity;
    public readonly AudioClipDataPlayer player;
    public readonly ActiveAudioReleaseMode releaseMode;
    
    public AudioHandle(Entity entity, AudioClipDataPlayer player, ActiveAudioReleaseMode releaseMode) {
        this.entity = entity;
        this.player = player;
        this.releaseMode = releaseMode;
    }
}

public enum ActiveAudioReleaseMode {
    MANUAL,
    ON_STATE_END,
}
}
