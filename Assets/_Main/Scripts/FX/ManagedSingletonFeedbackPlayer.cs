using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.FX {
public abstract class ManagedSingletonFeedbackPlayer<T> : SingletonBehaviour<T> where T: MonoBehaviour {
    protected MMF_Player CreatePlayer() {
        var go = new GameObject($"Player${Time.frameCount}");
        go.transform.SetParent(transform);

        var ret = go.AddComponent<MMF_Player>();
        ret.InitializationMode = MMFeedbacks.InitializationModes.Script;
        ret.PlayerTimescaleMode = TimescaleModes.Scaled;
        ret.Events.TriggerUnityEvents = true;

        ret.Events.OnComplete = new();
        ret.Events.OnComplete.AddListener(() => {
            Destroy(go);
        });
        
        return ret;
    }

}
}
