using System;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;
using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.FX;
using Unity.Cinemachine;
using UnityEngine;

namespace SuperSmashRhodes.Battle.FX {
public class SimpleCameraShakePlayer : ManagedSingletonFeedbackPlayer<SimpleCameraShakePlayer> {
    
    public void Play(SimpleCameraShakeData data) {
        var player = CreatePlayer();

        var shake = new MMF_CinemachineImpulse();
        var def = new CinemachineImpulseDefinition();
        def.ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Uniform;
        def.ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Custom;
        def.CustomImpulseShape = data.shakeCurve;
        def.ImpulseDuration = data.duration;
        shake.m_ImpulseDefinition = def;

        player.AddFeedback(shake);
        player.AutomaticShakerSetup();
        player.Initialization();
        
        
        shake.Velocity = data.velocity;
        
        player.PlayFeedbacks();
    }

    public void Play(string libraryPath, string shakeId) {
        AssetManager.Get<CameraShakeLibrary>(libraryPath, lib => {
            if (!lib.shakes.TryGetValue(shakeId, out var data)) {
                Debug.LogError($"Camera shake {shakeId} not found in library {libraryPath}");
                return;
            }
            Play(data);
        });
    }
}

[Serializable]
public struct SimpleCameraShakeData {
    public AnimationCurve shakeCurve;
    public Vector3 velocity;
    public float duration;
}
}
