using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_NmlAtk5S : AttackStateBase {
    public State_Common_NmlAtk5S(Entity owner) : base(owner) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_5CS;
    public override int inputPriority => 3;

    protected override string mainAnimation => "cmn_NmlAtk5S";

    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_SPECIAL_SUPER | 
                                                             EntityStateType.CHR_ATK_5S | EntityStateType.CHR_ATK_2S
                                                                                         | EntityStateType.CHR_ATK_6P
                                                                                         | EntityStateType.CHR_ATK_5H | EntityStateType.CHR_ATK_6H | EntityStateType.CHR_ATK_2H
                                                                                         | EntityStateType.CHR_ATK_5D | EntityStateType.CHR_ATK_2D;


    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.TimeSlice(6).ScanForInput(new InputFrame(InputType.S, InputFrameType.PRESSED));
    }
}
}
