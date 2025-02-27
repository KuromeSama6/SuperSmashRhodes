using System;
using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Framework;
using UnityEngine;

namespace SuperSmashRhodes.Scripts.Audio {
public class AudioManager : AutoInitSingletonBehaviour<AudioManager> {
    private int idCounter;
    private readonly Dictionary<string, ISoundHandle> channels = new();
    private readonly Dictionary<ISoundHandle, OwnedPlayerData> ownedPlayers = new();
    
    protected override void Awake() {
        base.Awake();
    }

    private void Start() {
    }

    private void Update() {
        foreach (var handle in ownedPlayers.Keys.ToList()) {
            var data = ownedPlayers[handle];
            
            if (!handle.valid || !data.valid) {
                ownedPlayers.Remove(handle);
                continue;
            }
            
        }
        
    }
    
    public void PlayAudioClip(string path, GameObject owner, string channel = null, float volume = 1f, float pitch = 1f, bool loop = false) {
        AssetManager.Get<AudioClip>(path, clip => {
            PlayAudioClip(clip, owner, channel, volume, pitch, loop);
        });
    }
    
    public ISoundHandle PlayAudioClip(AudioClip clip, GameObject owner, string channel = null, float volume = 1f, float pitch = 1f, bool loop = false) {
        ++idCounter;
        var go = new GameObject($"Audio${idCounter}");
        go.transform.parent = transform;

        var ret = go.AddComponent<AudioClipPlayer>();
        ret.Init(idCounter);
        ret.Play(clip, volume, pitch);
        
        if (channel != null) {
            if (channels.TryGetValue(channel, out var current)) {
                current.Release(0.1f);
            }
            
            channels[channel] = ret;
        }
        
        ownedPlayers[ret] = new OwnedPlayerData(owner);
        return ret;
    }
    
    public void PlayBGM(string path, GameObject owner, float fadeIn = .5f, float volume = 1f, float pitch = 1f) {
        AssetManager.Get<BGMLoopData>(path, data => PlayBGM(data, owner, fadeIn, volume, pitch));
    }
    
    public ISoundHandle PlayBGM(IAudioLoopData data, GameObject owner, float fadeIn = .5f, float volume = 1f, float pitch = 1f) {
        ++idCounter;
        var go = new GameObject($"BGM${idCounter}");
        go.transform.parent = transform;
        
        // release current bgm
        if (channels.TryGetValue("_bgm", out var current)) {
            current.Release(0.1f);
        }
        
        var ret = go.AddComponent<BGMLoopPlayer>();
        ret.Play(data, fadeIn, volume, pitch);
        channels["_bgm"] = ret;
        
        ownedPlayers[ret] = new OwnedPlayerData(owner);
        return ret;
    }

    public void StopChannel(string channel, float fadeOut = 0.1f) {
        if (channels.TryGetValue(channel, out var current)) {
            current.Release(fadeOut);
            channels.Remove(channel);
        }
    }

    public void StopBGM(float fadeOut = 0.1f) {
        StopChannel("_bgm", fadeOut);
    }
    
}

class OwnedPlayerData {
    public GameObject gameObject;
    public bool valid => gameObject;
    
    public OwnedPlayerData(GameObject owner) {
        gameObject = owner;
    }
}
}
