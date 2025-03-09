using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Runtime.State;

namespace SuperSmashRhodes.Runtime.Character {
[NamedToken("Rosmontis_NmlAtk5P")]
public class State_Rosmontis_NmlAtk5P : State_Common_NmlAtk5P {
    public State_Rosmontis_NmlAtk5P(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new(5, 4, 8);
    public override float GetUnscaledDamage(Entity to) {
        return 18;
    }
    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/fist/1";
    }
}

[NamedToken("Rosmontis_NmlAtk2P")]
public class State_Rosmontis_NmlAtk2P : State_Common_NmlAtk2P {
    public State_Rosmontis_NmlAtk2P(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new(6, 4, 9);
    public override float GetUnscaledDamage(Entity to) {
        return 18;
    }
    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/fist/1";
    }
}

[NamedToken("Rosmontis_NmlAtk8P")]
public class State_Rosmontis_NmlAtk8P : State_Common_NmlAtk8P {
    public State_Rosmontis_NmlAtk8P(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new(5, 4, 8);
    public override float GetUnscaledDamage(Entity to) {
        return 15;
    }
    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/fist/1";
    }
}
}
