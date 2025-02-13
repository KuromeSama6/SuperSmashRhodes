using System;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Scripts.Audio {
public class AudioClipDataPlayer : MonoBehaviour {
    private AudioSource audioSource;
    public AudioClipData data { get; private set; }
    public bool loop { get; set; }
    public bool isPlaying => audioSource.isPlaying;

    private void Awake() {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start() {
        audioSource.playOnAwake = true;
    }

    public void Play(AudioClipData clip) {
        data = clip;
        audioSource.clip = clip.clips.RandomChoice();
        audioSource.time = 0;
        audioSource.Play();
    }

    private void Update() {
        if (data != null && audioSource) {
            audioSource.volume = data.volumeCurve.Evaluate(audioSource.time / audioSource.clip.length);
            audioSource.pitch = data.pitchCurve.Evaluate(audioSource.time / audioSource.clip.length);
        }

        if (!audioSource.isPlaying) {
            if (loop) {
                audioSource.Play();
            } else {
                data = null;
                Destroy(gameObject);
            }
        }
    }
}
}
