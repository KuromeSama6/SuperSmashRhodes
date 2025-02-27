using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SuperSmashRhodes.Scripts.Audio {
[CreateAssetMenu(fileName = "New BGM Loop", menuName = "SSR/Audio/BGM Loop", order = 0)]
public class BGMLoopData : ScriptableObject, IAudioLoopData {
    [field: SerializeField]
    public AssetReferenceT<AudioClip> intro { get; set; }
    [field: SerializeField]
    public AssetReferenceT<AudioClip> loop { get; set; }
}
}
