using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Texas_NmlAtk5CS")]
public class StateTexasNmlAtk5CS : State_Common_NmlAtk5CS {
    public StateTexasNmlAtk5CS(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new() {
        startup = 7,
        active = 6,
        recovery = 10,
    };

    public override float GetUnscaledDamage(Entity to) {
        return 36f;
    }

    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/sword/1";
    }
}
}
