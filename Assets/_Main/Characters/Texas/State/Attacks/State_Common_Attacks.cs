using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Texas_NmlAtk5S")]
public class State_Texas_NmlAtk5S : State_Common_NmlAtk5S {
    public State_Texas_NmlAtk5S(Entity owner) : base(owner) { }
    public override AttackFrameData frameData => new() {
        startup = 7,
        active = 6,
        recovery = 10,
        onHit = +4,
        onBlock = +1,
    };
    public override AttackProperties attackProperties => new() {
        damage = 36,
        chipDamagePercentage = 0,
        otgDamagePercentage = 0,
        pushback = 2f,
        comboProration = 0.95f,
        firstHitProration = 1f,
        guardType = AttackGuardType.ALL,
        freezeFrames = 2
    };
}
}
