using System.Collections;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnNeutral")]
public class State_CmnNeutral : EntityState {
    public override int inputPriority => -1;
    public override EntityStateType type => EntityStateType.CHR_NEUTRAL;

    public State_CmnNeutral(Entity owner) : base(owner, "CmnNeutral") { }
    
    public override bool IsInputValid(InputBuffer buffer) {
        return false;
    }
    
    public override IEnumerator MainRoutine() {
        AddCancelOption(EntityStateType.ALL);
        owner.animation.AddUnmanagedAnimation("std_neutral", true, 0.2f);

        while (true) {
            SetScheduledAnimationFrames(1);
            yield return 1;
        }
        
    }
}
}
