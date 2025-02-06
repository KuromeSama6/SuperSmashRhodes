using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Input;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_NmlAtkGndThrow : ThrowAttackStateBase {
    public State_Common_NmlAtkGndThrow(Entity owner) : base(owner) { }
    public override float inputPriority => 4f;
    public override bool IsInputValid(InputBuffer buffer) {
        var frame = buffer.thisFrame;
        return (frame.HasInput(owner.side, InputType.FORWARD, InputFrameType.HELD) || frame.HasInput(owner.side, InputType.BACKWARD, InputFrameType.HELD)) && frame.HasInput(owner.side, InputType.D, InputFrameType.PRESSED);
    }
    protected override string mainAnimation => "cmn/NmlAtkGndThrow";
    protected override string whiffAnimation => "cmn/NmlAtkGndThrow_W";
    public override AttackFrameData frameData => new() {
        startup = 2,
        active = 3,
        recovery = 38
    };
    protected override float distanceRequirement => .7f;
    public override float GetUnscaledDamage(Entity to) {
        return 80f;
    }
    protected override bool ClashableWith(ThrowAttackStateBase other) {
        return other is State_Common_NmlAtkGndThrow;
    }
    protected override bool ShouldSwitchSides(PlayerCharacter other) {
        return player.inputProvider.inputBuffer.thisFrame.HasInput(owner.side, InputType.BACKWARD, InputFrameType.HELD);
    }

}
}
