using System;
using System.Collections;
using System.Collections.Generic;
using SuperSmashRhodes.Adressable;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SuperSmashRhodes.Scripts.Audio {
public class BGMLoopPlayer : MonoBehaviour, ISoundHandle {
    public int id { get; private set; }
    public AudioSource audioSource { get; private set; }
    public IAudioLoopData data { get; private set; }
    
    public bool valid => this;
    private bool introPlayed;
    private bool loopPlayed;

    private void Awake() {
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    public void Play(IAudioLoopData clip, float fadeIn = .5f, float volume = 1f, float pitch = 1f) {
        data = clip;
        
        AssetManager.inst.PreloadAll(clip.intro.RuntimeKey.ToString());
        AssetManager.inst.PreloadAll(clip.loop.RuntimeKey.ToString());
        
        AssetManager.Get(clip.intro, audioClip => {
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.Play();
            introPlayed = true;
        });
        
        if (fadeIn > 0) {
            StartCoroutine(FadeInCoroutine(volume, fadeIn));
        }
    }

    private void Update() {
        if (data == null || !audioSource) return;
        
        // replace with loop
        if (!audioSource.isPlaying && introPlayed && !loopPlayed) {
            AssetManager.Get(data.loop, audioClip => {
                audioSource.clip = audioClip;
                audioSource.loop = true;
                audioSource.time = 0;
                audioSource.Play();
            });
            
            loopPlayed = true;
        }
    }

    public void Release() {
        if (!audioSource) return;
        Destroy(audioSource.gameObject);
    }
    
    public void Release(float fadeOut, float delay = 0) {
        if (!audioSource) return;
        StartCoroutine(DelayedRelease(delay, fadeOut, audioSource));
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

    private IEnumerator FadeInCoroutine(float targetVolume, float duration) {
        audioSource.volume = 0f;
        
        while (audioSource.volume < targetVolume) {
            audioSource.volume += targetVolume * Time.deltaTime / duration;
            yield return null;
        }
    }
}

public interface IAudioLoopData {
    public AssetReferenceT<AudioClip> intro { get; }
    public AssetReferenceT<AudioClip> loop { get; }
}
}
