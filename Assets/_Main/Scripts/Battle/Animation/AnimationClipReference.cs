using System.Collections.Generic;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes._Main.Scripts.Battle.Animation {
[CreateAssetMenu(fileName = "NewAnimationClipReference", menuName = "Battle/Animation Clip Reference", order = 0)]
public class AnimationClipReference : ScriptableObject {
    public List<AnimationClip> clips = new();
}
}
