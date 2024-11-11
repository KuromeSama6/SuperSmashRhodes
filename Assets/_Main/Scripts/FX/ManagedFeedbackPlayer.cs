using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.FX {
public abstract class ManagedFeedbackPlayer : MonoBehaviour {
    protected MMF_Player CreatePlayer() {
        var go = new GameObject($"Player${Time.frameCount}");
        go.transform.SetParent(transform);

        var ret = go.AddComponent<MMF_Player>();
        ret.InitializationMode = MMFeedbacks.InitializationModes.Script;
        
        return ret;
    }
}
}
