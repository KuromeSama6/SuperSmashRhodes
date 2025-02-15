using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Exusiai_SklRoll")]
public class State_Exusiai_SklRoll : State_Common_UtilityMove {
    public State_Exusiai_SklRoll(Entity entity) : base(entity) { }
    public override float inputPriority => 5f;
    protected override string mainAnimation => "chr/SklRoll";
    protected override float inputMeter => 1f;
    public override AttackFrameData frameData => new AttackFrameData() {
        startup = 15, active = 6, recovery = 10
    };

    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_SUPER;
    protected override InputFrame[] requiredInput => new InputFrame[] {
        new(InputType.DOWN, InputFrameType.HELD),
        new(InputType.FORWARD, InputFrameType.PRESSED),
        new(InputType.S, InputFrameType.PRESSED),
    };

    protected override void OnActive() {
        base.OnActive();
        if (player.airborne) {
            player.ApplyForwardVelocity(new(10f, 0));
        } else {
            player.ApplyForwardVelocity(new(15f, 0));
        }
        
    }

    protected override void OnRecovery() {
        base.OnRecovery();
        AddCancelOption("Exusiai_SklRoll_FThrow");
        AddCancelOption("Exusiai_SklRoll_FSlide");
        AddCancelOption("Exusiai_SklRoll_FEvade");
    }

    protected override void OnStateEnd(string nextState) {
        base.OnStateEnd(nextState);
        player.ApplyGroundedFrictionImmediate();
    }
}
}
