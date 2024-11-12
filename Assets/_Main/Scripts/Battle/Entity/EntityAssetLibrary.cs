using System.Collections.Generic;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
[CreateAssetMenu(menuName = "Battle/Entity Asset Library")]
public class EntityAssetLibrary : ScriptableObject {
    public List<AudioClip> audioClips = new();

    public void MergeFrom(EntityAssetLibrary other) {
        audioClips.AddRange(other.audioClips);
    }

    public AudioClip GetAudioClip(string name) {
        var ret = audioClips.Find(clip => clip.name == name);
        if (!ret) throw new System.Exception($"Audio clip {name} not found");
        return ret;
    }
}
}
