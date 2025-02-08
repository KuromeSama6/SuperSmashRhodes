using System;
using System.Collections.Generic;
using SuperSmashRhodes.Adressable;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class EntityAudioManager : MonoBehaviour {
    private Entity entity;
    private AudioSource audioSource;

    private Dictionary<int, AudioSource> loopSources { get; } = new();
    private int idCounter;

    private void Start() {
        entity = GetComponent<Entity>();
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(string soundName, float volume = 1f) {
        if (soundName == null) return;
        AssetManager.Get<AudioClip>(soundName, clip => audioSource.PlayOneShot(clip, volume)); 
    }

    public int PlaySoundLoop(string soundName, float volume = 1f) {
        var id = idCounter++;
        AssetManager.Get<AudioClip>(soundName, clip => {
            var go = new GameObject($"LoopSource_{id}");
            go.transform.parent = transform.parent;
            
            var source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.volume = volume;
            source.Play();
            loopSources[id] = source;
        });
        return id;
    }

    public void StopSoundLoop(int id, string tailSound = null, float tailVolume = 1f) {
        if (loopSources.ContainsKey(id)) {
            Destroy(loopSources[id].gameObject);
            loopSources.Remove(id);
            
            if (tailSound != null) {
                PlaySound(tailSound, tailVolume);
            }
        }
    }
}
}
