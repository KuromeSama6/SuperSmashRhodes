using System;
using SuperSmashRhodes.Adressable;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class EntityAudioManager : MonoBehaviour {
    private Entity entity;
    private AudioSource audioSource;

    private void Start() {
        entity = GetComponent<Entity>();
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(string soundName, float volume = 1f) {
        if (soundName == null) return;
        AssetManager.Get<AudioClip>(soundName, clip => audioSource.PlayOneShot(clip, volume)); 
    }
}
}
