using UnityEngine;

namespace SuperSmashRhodes.Scripts.Audio {
[CreateAssetMenu(fileName = "New BGM Loop", menuName = "SSR/Audio/BGM Loop", order = 0)]
public class BGMLoopData : ScriptableObject {
    public AudioClip intro;
    public AudioClip loop;
}
}
