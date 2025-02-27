using System;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SuperSmashRhodes.Battle.Postprocess{
public class PostProcessManager : SingletonBehaviour<PostProcessManager> {
    [Title("References")]
    public Volume volume;

    [Title("Shaker References")]
    public UDictionary<string, MMShaker> shakers = new();

    public bool showKnockout { get; set; } = false;
    
    private Vignette vignette;
    private DepthOfField dof;
    private ColorAdjustments colorAdjustments;
    
    private void Start() {
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out dof);
        volume.profile.TryGet(out colorAdjustments);
    }

    private void Update() {
        if (colorAdjustments) {
            if (showKnockout) {
                colorAdjustments.contrast.value = 100f;
                colorAdjustments.saturation.value = -100f;
            }
        
            colorAdjustments.contrast.value = Mathf.Lerp(colorAdjustments.contrast.value, 0, Time.deltaTime * 2.5f);
            colorAdjustments.saturation.value = Mathf.Lerp(colorAdjustments.saturation.value, 0, Time.deltaTime * 2.5f);   
        }
    }

    public void PlayShaker(string key) {
        if (shakers.TryGetValue(key, out var shaker)) {
            shaker.enabled = true;
        } else {
            Debug.LogError($"Shaker with key {key} not found.");
        }
    }
}
}
