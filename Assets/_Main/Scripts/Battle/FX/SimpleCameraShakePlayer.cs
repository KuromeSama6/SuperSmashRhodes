using MoreMountains.FeedbacksForThirdParty;
using SuperSmashRhodes.FX;
using Unity.Cinemachine;

namespace SuperSmashRhodes.Battle.FX {
public class SimpleCameraShakePlayer : ManagedFeedbackPlayer {
    public void Play(SimpleCameraShakeData data) {
        var player = CreatePlayer();

        var shake = new MMF_CinemachineImpulse();
        shake.Velocity = data.velocity;

        var def = new CinemachineImpulseDefinition();
        def.ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Uniform;
        def.ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Custom;
        def.CustomImpulseShape = data.shakeCurve;
        shake.m_ImpulseDefinition = def;
        
        player.AutomaticShakerSetup();
        player.Initialization();
        player.PlayFeedbacks();
    }
}
}
