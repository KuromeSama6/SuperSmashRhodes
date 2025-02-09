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
    public State_Exusiai_NmlAtk5CS(Entity entity) : base(entity) { }
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
    public State_Exusiai_NmlAtk5S(Entity entity) : base(entity) { }
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
public class State_Exusiai_NmlAtk2S : State_Exusiai_MultihitWeaponAttack {
    public State_Exusiai_NmlAtk2S(Entity entity) : base(entity) { }
    protected override string mainAnimation => "cmn/NmlAtk2S";

    public override AttackFrameData frameData => new() {
        startup = 10,
        active = 11,
        recovery = 15,
        onHit = 0,
        onBlock = -7,
    };

    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_DRIVE_SPECIAL_SUPER | EntityStateType.CHR_ATK_NORMAL_H;
    protected override InputFrame[] requiredInput => new[] { new InputFrame(InputType.DOWN, InputFrameType.HELD), new InputFrame(InputType.S, InputFrameType.PRESSED)};
    protected override int normalInputBufferLength => 6;
    public override float GetUnscaledDamage(Entity to) {
        return 20;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(1.2f, 2f) * (blocked ? 0.7f : 1f);
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.CROUCHING;
    }
    public override EntityStateType type => EntityStateType.CHR_ATK_2S;
    public override float inputPriority => 3.1f;
    protected override string weaponSfx => "chr/exusiai/battle/sfx/gun_1";
    protected override int maxHits => 3;
    protected override int emptySkipFrame => 21;
    public override int GetFreezeFrames(Entity to) {
        return 3;
    }

    public override int GetAttackLevel(Entity to) {
        return 1;
    }
}

[NamedToken("Exusiai_NmlAtk5H")]
public class State_Exusiai_NmlAtk5H : State_Exusiai_MultihitWeaponAttack {
    public State_Exusiai_NmlAtk5H(Entity entity) : base(entity) { }
    protected override string mainAnimation => "cmn/NmlAtk5H";

    public override AttackFrameData frameData => new() {
        startup = 12,
        active = 12,
        recovery = 16,
        onHit = +5,
        onBlock = -6,
    };

    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_DRIVE_SPECIAL_SUPER;
    protected override InputFrame[] requiredInput => new[] { new InputFrame(InputType.HS, InputFrameType.PRESSED)};
    protected override int normalInputBufferLength => 6;
    public override float GetUnscaledDamage(Entity to) {
        return 31;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
         return new Vector2(1.5f, 1f) * (blocked ? 0.7f : 1f);
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override EntityStateType type => EntityStateType.CHR_ATK_5H;
    public override float inputPriority => 3;
    protected override string weaponSfx => "chr/exusiai/battle/sfx/gun_1";
    protected override int maxHits => 3;
    protected override int emptySkipFrame => 23;
    public override int GetFreezeFrames(Entity to) {
        return 3;
    }

    public override int GetAttackLevel(Entity to) {
        return 4;
    }
}

[NamedToken("Exusiai_NmlAtk2H")]
public class State_Exusiai_NmlAtk2H : State_Exusiai_MultihitWeaponAttack {
    public State_Exusiai_NmlAtk2H(Entity entity) : base(entity) { }
    protected override string mainAnimation => "cmn/NmlAtk2H";

    public override AttackFrameData frameData => new() {
        startup = 13,
        active = 17,
        recovery = 28,
        onHit = +10,
        onBlock = -18,
    };

    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_DRIVE_SPECIAL_SUPER;
    protected override InputFrame[] requiredInput => new[] { new InputFrame(InputType.DOWN, InputFrameType.HELD), new InputFrame(InputType.HS, InputFrameType.PRESSED)};
    protected override int normalInputBufferLength => 6;
    public override float GetUnscaledDamage(Entity to) {
        return 31;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(1.2f, 7f) * (blocked ? 0.7f : 1f);
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.CROUCHING;
    }
    public override EntityStateType type => EntityStateType.CHR_ATK_2H;
    public override float inputPriority => 3.1f;
    protected override string weaponSfx => "chr/exusiai/battle/sfx/gun_1";
    protected override int maxHits => 5;
    protected override int emptySkipFrame => 35;
    public override int GetFreezeFrames(Entity to) {
        return 3;
    }
    public override int GetAttackLevel(Entity to) {
        return 4;
    }
}

[NamedToken("Exusiai_NmlAtk6H")]
public class State_Exusiai_NmlAtk6H : State_Exusiai_MultihitWeaponAttack {
    private int audioHandle;
    public State_Exusiai_NmlAtk6H(Entity entity) : base(entity) {
        onFireStart.AddListener(() => audioHandle = entity.audioManager.PlaySoundLoop("chr/exusiai/battle/sfx/gun_loop", 0.5f));
        onFireEnd.AddListener(() => entity.audioManager.StopSoundLoop(audioHandle, "chr/exusiai/battle/sfx/gun_loop_tail", 0.5f));
    }
    protected override string mainAnimation => "cmn/NmlAtk6H";

    public override AttackFrameData frameData => new() {
        startup = 18,
        active = 20,
        recovery = 33,
        onHit = +22,
        onBlock = -16,
    };

    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_DRIVE_SPECIAL_SUPER;
    protected override InputFrame[] requiredInput => new[] { new InputFrame(InputType.FORWARD, InputFrameType.HELD), new InputFrame(InputType.HS, InputFrameType.PRESSED)};
    protected override int normalInputBufferLength => 6;
    public override float GetUnscaledDamage(Entity to) {
        return 38;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(1.5f, 7f) * (blocked ? 0.7f : 1f);
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override EntityStateType type => EntityStateType.CHR_ATK_6H;
    public override float inputPriority => 3.1f;
    protected override string weaponSfx => null;
    protected override int maxHits => 7;
    protected override int emptySkipFrame => 38;
    public override int GetFreezeFrames(Entity to) {
        return 5;
    }

    public override int GetAttackLevel(Entity to) {
        return 4;
    }
}

[NamedToken("Exusiai_NmlAtkGndThrow")]
public class State_Exusiai_NmlAtkGndThrow : State_Common_NmlAtkGndThrow {
    public State_Exusiai_NmlAtkGndThrow(Entity entity) : base(entity) { }
    protected override int animationLength => 59;
}

[NamedToken("Exusiai_NmlAtk5P")]
public class State_Exusiai_NmlAtk5P : State_Common_NmlAtk5P {

    public State_Exusiai_NmlAtk5P(Entity entity) : base(entity) { }
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
    public State_Exusiai_NmlAtk2P(Entity entity) : base(entity) { }
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

    public State_Exusiai_NmlAtk5D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_5D;
    public override float inputPriority => 3f;

    protected override string mainAnimation => "cmn/NmlAtk5D";
    protected override InputFrame[] requiredInput => new InputFrame[] {new(InputType.D, InputFrameType.PRESSED)};
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (bulletsShot > 7) {
            return new Vector2(1.7f, 1.5f);
        }
        return new Vector2(1.5f, 1f);
    }
}

[NamedToken("Exusiai_NmlAtk2D")]
public class State_Exusiai_NmlAtk2D : State_Exusiai_DriveAttack {
    public State_Exusiai_NmlAtk2D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_2D;
    public override float inputPriority => 3.1f;
    protected override string mainAnimation => "cmn/NmlAtk2D";
    protected override InputFrame[] requiredInput => new InputFrame[] {new (InputType.DOWN, InputFrameType.HELD), new(InputType.D, InputFrameType.PRESSED)};
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(0.7f, 2.3f) * (blocked ? 0.7f : 1f);
    }
}

[NamedToken("Exusiai_NmlAtk6D")]
public class State_Exusiai_NmlAtk6D : State_Exusiai_DriveAttack {
    public State_Exusiai_NmlAtk6D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_6D;
    public override float inputPriority => 3.1f;
    protected override string mainAnimation => "cmn/NmlAtk6D";
    protected override InputFrame[] requiredInput => new InputFrame[] {new (InputType.FORWARD, InputFrameType.HELD), new(InputType.D, InputFrameType.PRESSED)};
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (bulletsShot > 7 && !blocked) {
            return new Vector2(1f, 1.7f);
        }
        return new Vector2(1.3f, 1.5f) * (blocked ? 0.7f : 1f);
    }
}

[NamedToken("Exusiai_NmlAtk4D")]
public class State_Exusiai_NmlAtk4D : State_Exusiai_DriveAttack {
    public State_Exusiai_NmlAtk4D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_4D;
    public override float inputPriority => 3.1f;
    protected override string mainAnimation => "cmn/NmlAtk4D";
    protected override InputFrame[] requiredInput => new InputFrame[] {new (InputType.BACKWARD, InputFrameType.HELD), new(InputType.D, InputFrameType.PRESSED)};
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(2f, 0f) * (blocked ? 0.7f : 1f);
    }
}

}
