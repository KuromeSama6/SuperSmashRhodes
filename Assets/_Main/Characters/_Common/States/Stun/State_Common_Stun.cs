using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_Stun : CharacterState {
    protected State_Common_Stun(Entity entity) : base(entity) { }
    public override float inputPriority => -1;
    public override bool mayEnterState => false;
    public override bool IsInputValid(InputBuffer buffer) {
        return false;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        entity.animation.ApplyNeutralPose();
        entity.animation.AddUnmanagedAnimation(animationName, true, 0);
        
        AddCancelOption("CmnBurst");
        stateData.maySwitchSides = true;
    }

    public override IEnumerator MainRoutine() {
        while (frames > 0) {
            // Debug.Log("tick");
            player.ApplyGroundedFriction();
            // Debug.Log($"stun {frames}");

            if (!player.airborne && player.transform.position.y >= 0.2f) {
                CancelInto("CmnHitStunAir");
                yield break;
            }
            
            yield return 1;
        }
    }

    protected override void OnStateEnd(EntityState nextState) {
        base.OnStateEnd(nextState);
        player.frameData.throwInvulnFrames = 5;
    }

    // Abstract properties
    protected abstract int frames { get; }
    protected abstract string animationName { get; }
}
}
