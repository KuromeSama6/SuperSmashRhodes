using System;
using System.Collections;
using UnityEngine;

namespace SuperSmashRhodes.Scripts.Audio {
public class AudioClipPlayer : MonoBehaviour, ISoundHandle {
    public int id { get; private set; }
    public AudioSource audioSource { get; private set; }

    public bool valid => this;
    
    public void Init(int id) {
        this.id = id;
    }

    private void Awake() {
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    public void Play(AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = false, float time = 0) {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.time = time;
        audioSource.loop = loop;
        audioSource.Play();
    }

    private void Update() {
        if (audioSource && !audioSource.isPlaying) {
            Destroy(gameObject);
        }
    }

    public void Release() {
        if (!audioSource) return;
        Destroy(audioSource.gameObject);
    }
    
    public void Release(float fadeOut, float delay = 0) {
        if (!audioSource) return;
        StartCoroutine(DelayedRelease(delay, fadeOut, audioSource));
        audioSource = null;
    }

    private IEnumerator DelayedRelease(float delay, float fadeOut, AudioSource audioSource) {
        if (!audioSource) yield break;
        yield return new WaitForSeconds(delay);
        
        var startVolume = audioSource.volume;
        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / fadeOut;
            yield return null;
        }
        
        Destroy(audioSource.gameObject);
    }
}
}
