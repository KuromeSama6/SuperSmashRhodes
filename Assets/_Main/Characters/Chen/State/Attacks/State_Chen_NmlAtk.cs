using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Chen_NmlAtk5CS")]
public class State_Chen_NmlAtk5CS : State_Common_NmlAtk5CS {
    public State_Chen_NmlAtk5CS(Entity owner) : base(owner) { }
    public override AttackFrameData frameData => new() {
        startup = 7,
        active = 6,
        recovery = 10,
        onHit = +4,
        onBlock = +1,
    };

    public override float GetUnscaledDamage(Entity to) {
        return 41;
    }

    public override string GetAttackNormalSfx() {
        return "battle_generic_atk_sword1";
    }
}

[NamedToken("Chen_NmlAtk5S")]
public class State_Chen_NmlAtk5S : State_Common_NmlAtk5S {
    public State_Chen_NmlAtk5S(Entity owner) : base(owner) { }
    public override AttackFrameData frameData => new() {
        startup = 10,
        active = 3,
        recovery = 16,
        onHit = -2,
        onBlock = -5,
    };
    public override float GetUnscaledDamage(Entity to) {
        return 34;
    }
    public override string GetAttackNormalSfx() {
        return "battle_generic_atk_sword2";
    }
}


[NamedToken("Chen_NmlAtk5H")]
public class State_Chen_NmlAtk5H : State_Common_NmlAtk5H {
    public State_Chen_NmlAtk5H(Entity owner) : base(owner) { }
    public override AttackFrameData frameData => new() {
        startup = 12,
        active = 6,
        recovery = 21,
        onHit = -5,
        onBlock = -8,
    };
    public override float GetUnscaledDamage(Entity to) {
        return 48;
    }
    public override string GetAttackNormalSfx() {
        return "battle_generic_atk_sword3";
    }
}

[NamedToken("Chen_NmlAtk2H")]
public class State_Chen_NmlAtk2H : State_Common_NmlAtk2H {
    public State_Chen_NmlAtk2H(Entity owner) : base(owner) { } 
    public override AttackFrameData frameData => new() {
        startup = 11,
        active = 5,
        recovery = 28,
        onHit = +2,
        onBlock = -13,
    };
    public override float GetUnscaledDamage(Entity to) {
        return 54;
    }
    public override string GetAttackNormalSfx() {
        return "battle_generic_atk_sword9";
    }
}
}
