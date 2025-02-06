using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnWhiteForceReset")]
public class State_CmnWhiteForceReset : CharacterState {
    public State_CmnWhiteForceReset(Entity owner) : base(owner) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_SYSTEMSPECIAL;
    public override float inputPriority => 20f;
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.TimeSlice(6).ScanForInput(owner.side, new InputFrame(InputType.FORCE_RESET, InputFrameType.PRESSED));
    }
    public override bool mayEnterState => player.meter.gauge.value >= 50f && player.activeState is CharacterAttackStateBase;

    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.meter.gauge.value -= 50f;
        player.ApplyGroundedFrictionImmediate();
        opponent.comboCounter.comboDecay = 0f;
        player.fxManager.PlayGameObjectFX("cmn/batte/fx/prefab/common/force_reset/0", CharacterFXSocketType.SELF);
        player.audioManager.PlaySound("cmn/battle/sfx/force_reset/0");
    }

    public override IEnumerator MainRoutine() {
        yield break;
    }
}
}
