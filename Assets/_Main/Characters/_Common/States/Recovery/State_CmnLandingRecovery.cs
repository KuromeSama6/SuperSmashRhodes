using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnLandingRecovery")]
public class State_CmnLandingRecovery : CharacterState {
    public State_CmnLandingRecovery(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_COMMON_RECOVERY;
    public override float inputPriority { get; }
    public override Hitstate hitstate => Hitstate.PUNISH;

    public override bool IsInputValid(InputBuffer buffer) {
        return false;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        
        var animationName = stateData.carriedLandingAnimation ?? "std/land";
        // Debug.Log(player.frameData.landingRecoveryFrames);
        
        if (player.frameData.landingRecoveryFrames <= 0) {
            player.frameData.landingRecoveryFrames = 6;
        }
        
        player.animation.AddUnmanagedAnimation(animationName, false);
    }
    
    public override IEnumerator MainRoutine() {
        while (player.frameData.landingRecoveryFrames > 0) {
            --player.frameData.landingRecoveryFrames;
            yield return 1;
        }
    }
}
}
