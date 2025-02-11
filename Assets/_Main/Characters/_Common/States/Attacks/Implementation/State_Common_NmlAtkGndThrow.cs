using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Input;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_NmlAtkGndThrow : ThrowAttackStateBase {
    public State_Common_NmlAtkGndThrow(Entity entity) : base(entity) { }
    public override float inputPriority => 5f;
    public override bool IsInputValid(InputBuffer buffer) {
        // return buffer.TimeSlice(3).ScanForInput(owner.side, new InputFrame(InputType.D, InputFrameType.PRESSED), new InputFrame(InputType.P, InputFrameType.PRESSED)); 
        return buffer.TimeSlice(3).HasInputUnordered(entity.side, new InputFrame(InputType.D, InputFrameType.PRESSED), new InputFrame(InputType.P, InputFrameType.PRESSED)); 
    }
    protected override string mainAnimation => "cmn/NmlAtkGndThrow";
    protected override string whiffAnimation => "cmn/NmlAtkGndThrow_W";
    protected override float inputMeter => 0;

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
        return player.inputProvider.inputBuffer.thisFrame.HasInput(entity.side, InputType.BACKWARD, InputFrameType.HELD);
    }

}
}
