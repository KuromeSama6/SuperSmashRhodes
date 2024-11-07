using Unity.Cinemachine;
using UnityEngine;

namespace SuperSmashRhodes.CameraControl.Extension {
public class CinemachineLockYAxis : CinemachineExtension {
    public float yPosition = 0;
    
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime) {
        if (stage == CinemachineCore.Stage.Body)
        {
            var pos = state.RawPosition;
            pos.y = yPosition;
            state.RawPosition = pos;
        }
    }
}
}
