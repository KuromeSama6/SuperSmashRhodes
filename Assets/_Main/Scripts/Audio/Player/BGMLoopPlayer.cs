using System;
using UnityEngine;

namespace SuperSmashRhodes.Scripts.Audio {
public class BGMLoopPlayer : MonoBehaviour, ISoundHandle {
    public int id { get; private set; }
    public AudioSource audioSource { get; private set; }
    public BGMLoopData data { get; private set; }
    
    public bool valid => this;

    private void Awake() {
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    public void Play(BGMLoopData clip, float volume = 1f, float pitch = 1f) {
        data = clip;
        audioSource.clip = clip.intro;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();
    }

    private void Update() {
        if (!data || !audioSource) return;
        
        // replace with loop
        if (!audioSource.isPlaying) {
            audioSource.clip = data.loop;
            audioSource.loop = true;
            audioSource.time = 0;
            audioSource.Play();
        }
    }

    public void Release() {
        if (!audioSource) return;
        Destroy(audioSource.gameObject);
    }
    public void Release(float fadeOut, float delay = 0) {
        Release();
    }
}
}
