using Sirenix.OdinInspector;
using SuperSmashRhodes.Scripts.Audio;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SuperSmashRhodes.Battle.Stage {
[CreateAssetMenu(fileName = "New BGM Data", menuName = "SSR/Battle/BGM Data", order = 0)]
public class StageBGMData : ScriptableObject, IAudioLoopData {
    [Title("Metadata")]
    public string title;
    public string author;
    
    [field: Title("Sound References")]
    [field: SerializeField]
    public AssetReferenceT<AudioClip> intro { get; set; }
    [field: SerializeField]
    public AssetReferenceT<AudioClip> loop { get; set; }
}
}
