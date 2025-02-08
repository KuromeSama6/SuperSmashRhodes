using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Exusiai_NmlAtk5CS")]
public class State_Exusiai_NmlAtk5CS : State_Common_NmlAtk5CS {
    public State_Exusiai_NmlAtk5CS(Entity owner) : base(owner) { }
    public override AttackFrameData frameData => new() {
        startup = 8,
        active = 6,
        recovery = 12,
        onHit = +4,
        onBlock = +2,
    };

    public override float GetUnscaledDamage(Entity to) {
        return 43;
    }

    public override string GetAttackNormalSfx() {
        return $"cmn/battle/sfx/attack/fist/1";
    }
}

[NamedToken("Exusiai_NmlAtk5S")]
public class State_Exusiai_NmlAtk5S : State_Common_NmlAtk5S {
    public State_Exusiai_NmlAtk5S(Entity owner) : base(owner) { }
    public override AttackFrameData frameData => new() {
        startup = 10,
        active = 3,
        recovery = 19,
        onHit = -4,
        onBlock = -7,
    };
    public override float GetUnscaledDamage(Entity to) {
        return 37;
    }
    public override string GetAttackNormalSfx() {
        return $"cmn/battle/sfx/attack/fist/1";
    }
}

[NamedToken("Exusiai_NmlAtk2S")]
public class State_Exusiai_NmlAtk2S : State_Common_NmlAtk2S {
    public State_Exusiai_NmlAtk2S(Entity owner) : base(owner) { }
    public override AttackFrameData frameData => new() {
        startup = 11,
        active = 3,
        recovery = 20,
        onHit = -5,
        onBlock = -8,
    };
    public override float GetUnscaledDamage(Entity to) {
        return 32;
    }
    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/sword/2";
    }
}


[NamedToken("Exusiai_NmlAtk5H")]
public class State_Exusiai_NmlAtk5H : State_Common_NmlAtk5H {
    public State_Exusiai_NmlAtk5H(Entity owner) : base(owner) { }
    public override AttackFrameData frameData => new() {
        startup = 12,
        active = 6,
        recovery = 21,
        onHit = +5,
        onBlock = -8,
    };
    public override float GetUnscaledDamage(Entity to) {
        return 48;
    }
    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/sword/3";
    }
}

[NamedToken("Exusiai_NmlAtk2H")]
public class State_Exusiai_NmlAtk2H : State_Common_NmlAtk2H {
    public State_Exusiai_NmlAtk2H(Entity owner) : base(owner) { } 
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
        return "cmn/battle/sfx/attack/sword/9";
    }
}

[NamedToken("Exusiai_NmlAtkGndThrow")]
public class State_Exusiai_NmlAtkGndThrow : State_Common_NmlAtkGndThrow {
    public State_Exusiai_NmlAtkGndThrow(Entity owner) : base(owner) { }
    protected override int animationLength => 71;
    protected override int[] GetCosmeticHitFrames(PlayerCharacter to) {
        return new[] { 39 };
    }
}

[NamedToken("Exusiai_NmlAtk5P")]
public class State_Exusiai_NmlAtk5P : State_Common_NmlAtk5P {

    public State_Exusiai_NmlAtk5P(Entity owner) : base(owner) { }
    public override AttackFrameData frameData => new() {
        startup = 5,
        active = 3,
        recovery = 9,
        onHit = +1,
        onBlock = -2,
    };
    public override float GetUnscaledDamage(Entity to) {
        return 21f;
    }

    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/fist/1";
    }
}

[NamedToken("Exusiai_NmlAtk2P")]
public class State_Exusiai_NmlAtk2P : State_Common_NmlAtk2P {
    public State_Exusiai_NmlAtk2P(Entity owner) : base(owner) { }
    public override AttackFrameData frameData => new() {
        startup = 4,
        active = 2,
        recovery = 10,
        onHit = +1,
        onBlock = -2,
    };
    public override float GetUnscaledDamage(Entity to) {
        return 18f;
    }

    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/fist/1";
    }
}

[NamedToken("Exusiai_NmlAtk5D")]
public class State_Exusiai_NmlAtk5D : State_Exusiai_DriveAttack {

    public State_Exusiai_NmlAtk5D(Entity owner) : base(owner) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_5D;
    public override float inputPriority => 3;
    protected override string mainAnimation => "cmn/NmlAtk5D";
    public override AttackFrameData frameData { get; } = new AttackFrameData() {
        startup = 16,
        active = 2,
        recovery = 29,
    };

    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_2D | EntityStateType.CHR_ATK_SPECIAL_SUPER;
    protected override InputFrame[] requiredInput => new InputFrame[] {new(InputType.D, InputFrameType.PRESSED)};
    
    public override float GetUnscaledDamage(Entity to) {
        return 25;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (bulletsShot > 7) {
            return new Vector2(1.7f, 1.5f);
        }
        return new Vector2(1.5f, 1f);
    }

    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }

    protected override int loopStartFrame => 16;
    protected override int loopEndFrame => 19;
}

}
