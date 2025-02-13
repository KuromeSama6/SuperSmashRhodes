using System.Collections.Generic;
using UnityEngine;

namespace SuperSmashRhodes.Scripts.Audio {
[CreateAssetMenu(fileName = "New Audio Clip Data", menuName = "SSR/Audio/Clip Data", order = 0)]
public class AudioClipData : ScriptableObject {
    public List<AudioClip> clips = new();
    public AnimationCurve volumeCurve = AnimationCurve.Linear(0, 1, 1, 1);
    public AnimationCurve pitchCurve = AnimationCurve.Linear(0, 1, 1, 1); 
}
}
