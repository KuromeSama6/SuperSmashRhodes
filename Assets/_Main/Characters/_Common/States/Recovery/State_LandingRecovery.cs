using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnNeutralLandingRecovery")]
public class State_NeutralLandingRecovery : CharacterState {
    public State_NeutralLandingRecovery(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_COMMON_RECOVERY;
    public override float inputPriority { get; }
    public override bool IsInputValid(InputBuffer buffer) {
        return false;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.animation.AddUnmanagedAnimation("std/land", false);
        // AddCancelOption("CmnBackdash");
    }
    
    public override IEnumerator MainRoutine() {
        yield return 6;
    }
}
}
