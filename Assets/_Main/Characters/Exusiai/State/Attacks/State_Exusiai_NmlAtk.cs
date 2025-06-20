﻿using System.Collections;
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
    };
    public override float GetUnscaledDamage(Entity to) {
        return 37;
    }
    public override string GetAttackNormalSfx() {
        return $"cmn/battle/sfx/attack/fist/1";
    }
}

[NamedToken("Exusiai_NmlAtk2S")]
public class State_Exusiai_NmlAtk2S : State_Exusiai_MultihitWeaponNormalAttack {
    public State_Exusiai_NmlAtk2S(Entity entity) : base(entity) { }
    protected override string mainAnimation => "cmn/NmlAtk2S";

    public override AttackFrameData frameData => new() {
        startup = 10,
        active = 11,
        recovery = 15,
    };
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
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.MEDIUM;
    }
}

[NamedToken("Exusiai_NmlAtk5H")]
public class State_Exusiai_NmlAtk5H : State_Exusiai_MultihitWeaponNormalAttack {
    public State_Exusiai_NmlAtk5H(Entity entity) : base(entity) { }
    protected override string mainAnimation => "cmn/NmlAtk5H";

    public override AttackFrameData frameData => new() {
        startup = 12,
        active = 12,
        recovery = 16,
    };
    protected override InputFrame[] requiredInput => new[] { new InputFrame(InputType.HS, InputFrameType.PRESSED)};
    protected override int normalInputBufferLength => 6;
    public override float GetUnscaledDamage(Entity to) {
        return 19;
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
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.MEDIUM;
    }
}

[NamedToken("Exusiai_NmlAtk2H")]
public class State_Exusiai_NmlAtk2H : State_Exusiai_MultihitWeaponNormalAttack {
    public State_Exusiai_NmlAtk2H(Entity entity) : base(entity) { }
    protected override string mainAnimation => "cmn/NmlAtk2H";

    public override AttackFrameData frameData => new() {
        startup = 13,
        active = 17,
        recovery = 28,
    };
    protected override InputFrame[] requiredInput => new[] { new InputFrame(InputType.DOWN, InputFrameType.HELD), new InputFrame(InputType.HS, InputFrameType.PRESSED)};
    protected override int normalInputBufferLength => 6;
    public override float GetUnscaledDamage(Entity to) {
        return 19;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(0.2f, 9f) * (blocked ? 0.7f : 1f);
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
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.LARGE;
    }
}

[NamedToken("Exusiai_NmlAtk6H")]
public class State_Exusiai_NmlAtk6H : State_Exusiai_MultihitWeaponNormalAttack {
    private int audioHandle;
    public State_Exusiai_NmlAtk6H(Entity entity) : base(entity) {
        onFireStart.AddListener(() => audioHandle = entity.PlaySound("chr/exusiai/battle/sfx/gun_loop", 0.5f, true));
        onFireEnd.AddListener(() => entity.StopSound(audioHandle, "chr/exusiai/battle/sfx/gun_loop_tail", 0.5f));
    }
    protected override string mainAnimation => "cmn/NmlAtk6H";

    public override AttackFrameData frameData => new() {
        startup = 18,
        active = 20,
        recovery = 33,
    };
    protected override InputFrame[] requiredInput => new[] { new InputFrame(InputType.FORWARD, InputFrameType.HELD), new InputFrame(InputType.HS, InputFrameType.PRESSED)};
    protected override int normalInputBufferLength => 6;
    public override float GetUnscaledDamage(Entity to) {
        return 19;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(1f, 7.5f) * (blocked ? 0.7f : 1f);
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
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.LARGE;
    }

    public override float GetAtWallPushbackMultiplier(Entity to) {
        return 0.2f;
    }
}

[NamedToken("Exusiai_NmlAtkGndThrow")]
public class State_Exusiai_NmlAtkGndThrow : State_Common_NmlAtkGndThrow {
    public State_Exusiai_NmlAtkGndThrow(Entity entity) : base(entity) { }
    protected override int animationLength => 59;
}

[NamedToken("Exusiai_NmlAtkAirThrow")]
public class State_Exusiai_NmlAtkAirThrow : State_Common_NmlAtkAirThrow {
    public State_Exusiai_NmlAtkAirThrow(Entity entity) : base(entity) { }
    protected override int animationLength => 59;
}

[NamedToken("Exusiai_NmlAtk5P")]
public class State_Exusiai_NmlAtk5P : State_Common_NmlAtk5P {

    public State_Exusiai_NmlAtk5P(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new() {
        startup = 5,
        active = 3,
        recovery = 9,
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
    };
    public override float GetUnscaledDamage(Entity to) {
        return 18f;
    }

    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/fist/1";
    }
}

[NamedToken("Exusiai_NmlAtk8P")]
public class State_Exusiai_NmlAtk8P : State_Common_NmlAtk8P { 
    public State_Exusiai_NmlAtk8P(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new() {
        startup = 6,
        active = 3,
        recovery = 9,
    };
    public override float GetUnscaledDamage(Entity to) {
        return 20f;
    }

    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/fist/1";
    }
}

[NamedToken("Exusiai_NmlAtk8S")]
public class State_Exusiai_NmlAtk8S : State_Common_NmlAtk8S { 
    public State_Exusiai_NmlAtk8S(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new() {
        startup = 7,
        active = 4,
        recovery = 8,
    };
    public override float GetUnscaledDamage(Entity to) {
        return 26f;
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

    public override void OnHit(Entity target) {
        base.OnHit(target);
        AddCancelOption("CmnJump");
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
        return new Vector2(0.7f, 2.3f);
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

[NamedToken("Exusiai_NmlAtk8D")]
public class State_Exusiai_NmlAtk8D : State_Exusiai_DriveAttack {
    public State_Exusiai_NmlAtk8D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_8D;
    public override float inputPriority => 3f;

    protected override string mainAnimation => "cmn/NmlAtk8D";
    protected override InputFrame[] requiredInput => new InputFrame[] {new(InputType.D, InputFrameType.PRESSED)};
    protected override AttackAirOkType airOk => AttackAirOkType.AIR;
    public override LandingRecoveryFlag landingRecoveryFlag => LandingRecoveryFlag.UNTIL_LAND;
    public override AttackFrameData frameData => new AttackFrameData() {
        startup = 16,
        active = 2,
        recovery = 12
    };

    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (bulletsShot > 7) {
            return new Vector2(1.7f, 1.5f);
        }
        return new Vector2(1.5f, 1f);
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
    }

    protected override void OnActive() {
        base.OnActive();
        stateData.gravityScale = 0;
        player.rb.linearVelocity = Vector2.zero;
    }

    protected override void OnRecovery() {
        base.OnRecovery();
        stateData.gravityScale = 1;
    }

    protected override void OnShotFired() {
        base.OnShotFired();
        player.rb.linearVelocity = Vector2.zero;
        player.rb.AddForce(player.TranslateDirectionalForce(new(-1, 0)), ForceMode2D.Impulse);
    }
}

[NamedToken("Exusiai_NmlAtk82D")]
public class State_Exusiai_NmlAtk82D : State_Exusiai_DriveAttack {
    public State_Exusiai_NmlAtk82D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_82D;
    public override float inputPriority => 3.1f;
    protected override string mainAnimation => "cmn/NmlAtk82D";
    protected override InputFrame[] requiredInput => new InputFrame[] {new (InputType.DOWN, InputFrameType.HELD), new(InputType.D, InputFrameType.PRESSED)};
    protected override AttackAirOkType airOk => AttackAirOkType.AIR;
    public override LandingRecoveryFlag landingRecoveryFlag => LandingRecoveryFlag.UNTIL_LAND;
    public override AttackFrameData frameData => new AttackFrameData() {
        startup = 16,
        active = 2,
        recovery = 20
    };

    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (bulletsShot > 7) {
            return new Vector2(1.7f, 1.5f);
        }
        return new Vector2(1.5f, 1f);
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
    }

    protected override void OnActive() {
        base.OnActive();
        stateData.gravityScale = 0;
        player.rb.linearVelocity = Vector2.zero;
    }

    protected override void OnRecovery() {
        base.OnRecovery();
        stateData.gravityScale = 1;
    }

    protected override void OnShotFired() {
        base.OnShotFired();
        player.rb.linearVelocity = Vector2.zero;
        player.rb.AddForce(player.TranslateDirectionalForce(new(-1, 1)), ForceMode2D.Impulse);
    }

}


}
