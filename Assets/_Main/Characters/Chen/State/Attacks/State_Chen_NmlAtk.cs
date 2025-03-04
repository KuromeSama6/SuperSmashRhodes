using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using SuperSmashRhodes.Util;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Chen_NmlAtk5CS")]
public class State_Chen_NmlAtk5CS : State_Common_NmlAtk5CS {
    public State_Chen_NmlAtk5CS(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new() {
        startup = 7,
        active = 6,
        recovery = 10,
    };

    public override float GetUnscaledDamage(Entity to) {
        return 41;
    }

    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/sword/1";
    }
}

[NamedToken("Chen_NmlAtk5H")]
public class State_Chen_NmlAtk5H : State_Common_NmlAtk5H {
    public State_Chen_NmlAtk5H(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new() {
        startup = 12,
        active = 6,
        recovery = 21,
    };
    public override float GetUnscaledDamage(Entity to) {
        return 48;
    }
    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/sword/2";
    }
    public override void OnHit(Entity target) {
        base.OnHit(target);
        if (target is PlayerCharacter c) {
            Vector3 angle = new(0f, 0f, Random.Range(20, 80));
            Vector3 offset = new(Random.Range(-.1f, .1f), Random.Range(-.1f, .1f), 0);
            c.PlayFx($"chr/chen/battle/fx/prefab/nml/slash/1", CharacterFXSocketType.SELF, offset, angle);
            // Debug.Log("On hit");
            // AddressablesUtil.LoadAsync<GameObject>("chr/chen/battle/fx/p_chen_slash", go => {
            //     c.fxManager.PlayGameObjectFX(go, CharacterFXSocketType.SELF);
            // });
        }
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (airborne) new Vector2(4f, 4f);
        return new(blocked ? 3.5f : 2f, 0f);
    }
}

[NamedToken("Chen_NmlAtk2S")]
public class State_Chen_NmlAtk2S : State_Common_NmlAtk2S {
    public State_Chen_NmlAtk2S(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new() {
        startup = 11,
        active = 3,
        recovery = 20,
    };
    public override float GetUnscaledDamage(Entity to) {
        return 32;
    }
    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/sword/2";
    }
    public override void OnHit(Entity target) {
        base.OnHit(target);
        if (target is PlayerCharacter c) {
            Vector3 angle = new(0f, 0f, Random.Range(20, 80));
            Vector3 offset = new(Random.Range(-.1f, .1f), Random.Range(-.1f, .1f), 0);
            c.PlayFx("chr/chen/battle/fx/prefab/nml/slash/1", CharacterFXSocketType.SELF, offset, angle);
        }
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (airborne) return new Vector2(2f, 2.5f);
        return new(blocked ? 2.5f : .5f, 0);
    }
}

[NamedToken("Chen_NmlAtk5S")]
public class State_Chen_NmlAtk5S : State_Common_NmlAtk5S {
    public State_Chen_NmlAtk5S(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new() {
        startup = 10,
        active = 5,
        recovery = 13,
    };

    protected override void OnStartup() {
        base.OnStartup();
    }

    protected override void OnActive() {
        player.ApplyForwardVelocity(new(3.5f, 0f));
        base.OnActive();
        player.audioManager.PlaySoundClip("cmn/battle/sfx/generic/generic_swoosh_a");
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/dash_dust", CharacterFXSocketType.WORLD_UNBOUND, player.transform.position, Vector3.zero, new Vector3(player.side == EntitySide.LEFT ? 1 : -1, 1, 1));
    }

    public override float GetUnscaledDamage(Entity to) {
        return 32;
    }

    public override Vector2 GetCarriedMomentumPercentage(Entity to) {
        return Vector2.one;
    }
}

[NamedToken("Chen_NmlAtk6S")]
public class State_Chen_NmlAtk6S : State_Common_NmlAtk6S {
    public State_Chen_NmlAtk6S(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new() {
        startup = 12,
        active = 3,
        recovery = 21,
    };

    protected override void OnStartup() {
        base.OnStartup();
        player.audioManager.PlaySoundClip("cmn/battle/sfx/generic/generic_swoosh_a");
    }

    protected override void OnActive() {
        player.ApplyForwardVelocity(new(3f, 5f));
        base.OnActive();
    }

    public override float GetUnscaledDamage(Entity to) {
        return 26;
    }

    public override Vector2 GetCarriedMomentumPercentage(Entity to) {
        return Vector2.one;
    }
}


[NamedToken("Chen_NmlAtk6H")]
public class State_Chen_NmlAtk6H : State_Common_NmlAtk6H {
    public State_Chen_NmlAtk6H(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new() {
        startup = 15,
        active = 6,
        recovery = 26,
    };

    protected override void OnStartup() {
        base.OnStartup();
        // player.ApplyForwardVelocity(new(4.5f, 0f));
    }

    protected override void OnActive() {
        base.OnActive();
    }
    public override float GetUnscaledDamage(Entity to) {
        return 52;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (airborne) return new Vector2(blocked ? 6f : .5f, 4f);
        return blocked ? new Vector2(3f, 0f) : new(0, -5);
    }
    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/sword/3";
    }
    public override void OnHit(Entity target) {
        base.OnHit(target);
        if (target is PlayerCharacter c) {
            // c.PlayFx("p_chen_slash", CharacterFXSocketType.SELF);
            Vector3 angle = new(0f, 0f, Random.Range(20, 80));
            Vector3 offset = new(Random.Range(-.1f, .1f), Random.Range(-.1f, .1f), 0);
            c.PlayFx("chr/chen/battle/fx/prefab/nml/slash/1", CharacterFXSocketType.SELF, offset, angle);

            if (target is PlayerCharacter p && !p.airborne) {
                p.frameData.AddGroundBounce(new Vector2(.2f, 12f));
                p.ForceSetAirborne();
            }
        }
    }

    public override Vector2 GetCarriedMomentumPercentage(Entity to) {
        return Vector2.one;
    }
}

[NamedToken("Chen_NmlAtk2H")]
public class State_Chen_NmlAtk2H : State_Common_NmlAtk2H {
    public State_Chen_NmlAtk2H(Entity entity) : base(entity) { } 
    public override AttackFrameData frameData => new() {
        startup = 11,
        active = 5,
        recovery = 28,
    };
    public override float GetUnscaledDamage(Entity to) {
        return 54;
    }
    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/sword/9";
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (airborne) return new Vector2(2.5f, 5f);
        return blocked ? new(4f, 0f) : new Vector2(.5f, 12f);
    }
}

[NamedToken("Chen_NmlAtkGndThrow")]
public class State_Chen_NmlAtkGndThrow : State_Common_NmlAtkGndThrow {
    public State_Chen_NmlAtkGndThrow(Entity entity) : base(entity) { }
    protected override int animationLength => 71;
}
[NamedToken("Chen_NmlAtkAirThrow")]
public class State_Chen_NmlAtkAirThrow : State_Common_NmlAtkAirThrow {
    public State_Chen_NmlAtkAirThrow(Entity entity) : base(entity) { }
    protected override int animationLength => 71;
}

[NamedToken("Chen_NmlAtk5P")]
public class State_Chen_NmlAtk5P : State_Common_NmlAtk5P {

    public State_Chen_NmlAtk5P(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new() {
        startup = 5,
        active = 4,
        recovery = 7,
    };
    public override float GetUnscaledDamage(Entity to) {
        return 25f;
    }

    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/fist/1";
    }
}

[NamedToken("Chen_NmlAtk2P")]
public class State_Chen_NmlAtk2P : State_Common_NmlAtk2P {
    public State_Chen_NmlAtk2P(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new() {
        startup = 5,
        active = 4,
        recovery = 7,
    };
    public override float GetUnscaledDamage(Entity to) {
        return 21f;
    }

    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/fist/1";
    }
}

[NamedToken("Chen_NmlAtk8P")]
public class State_Chen_NmlAtk8P : State_Common_NmlAtk8P {
    public State_Chen_NmlAtk8P(Entity entity) : base(entity) { }

    public override AttackFrameData frameData => new() {
        startup = 5,
        active = 3,
        recovery = 8,

    };
    public override float GetUnscaledDamage(Entity to) {
        return 24;
    }
    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/fist/1";
    }
}

[NamedToken("Chen_NmlAtk8S")]
public class State_Chen_NmlAtk8S : State_Common_NmlAtk8S {
    public State_Chen_NmlAtk8S(Entity entity) : base(entity) { }

    public override AttackFrameData frameData => new() {
        startup = 10,
        active = 3,
        recovery = 23,

    };
    public override float GetUnscaledDamage(Entity to) {
        return 36;
    }
    
    public override string GetAttackNormalSfx() {
        return "cmn/battle/sfx/attack/sword/2";
    }
}



}
