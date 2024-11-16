using System.Collections.Generic;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
[CreateAssetMenu(menuName = "SSR/Battle/Entity Asset Library")]
public class EntityAssetLibrary : ScriptableObject {
    public List<AudioClip> audioClips = new();
    public List<GameObject> particles = new();

    public void MergeFrom(EntityAssetLibrary other) {
        audioClips.AddRange(other.audioClips);
        particles.AddRange(other.particles);
    }

    public AudioClip GetAudioClip(string name) {
        var ret = audioClips.Find(clip => clip.name == name);
        if (!ret) throw new System.Exception($"Audio clip {name} not found");
        return ret;
    }

    public GameObject GetParticle(string name) {
        // Debug.Log(string.Join(", ", particles)); 
        var ret = particles.Find(particle => particle.name == name);
        if (!ret) throw new System.Exception($"Particle {name} not found");
        return ret;
    }
}
}
