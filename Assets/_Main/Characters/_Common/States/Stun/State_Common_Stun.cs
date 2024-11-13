using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_Stun : CharacterState {
    protected State_Common_Stun(Entity owner) : base(owner) { }
    public override float inputPriority => -1;
    public override bool mayEnterState => false;
    public override bool IsInputValid(InputBuffer buffer) {
        return false;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        owner.animation.AddUnmanagedAnimation(animationName, true);
        // Debug.Log($"stun start, {frames}");
    }

    public override IEnumerator MainRoutine() {
        while (frames > 0) {
            player.ApplyGroundedFriction();
            // Debug.Log($"stun {frames}");
            yield return 1;
        }
    }

    // Abstract properties
    protected abstract int frames { get; }
    protected abstract string animationName { get; }
}
}
