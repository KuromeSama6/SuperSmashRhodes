using System;
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
        audioSource.PlayOneShot(entity.assetLibrary.GetAudioClip(soundName), volume);
    }
}
}
