using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Runtime.Gauge;
using SuperSmashRhodes.Runtime.State;
using UnityEngine;

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

[NamedToken("Rosmontis_NmlAtk5CS")]
public class State_Rosmontis_NmlAtk5CS : State_Common_NmlAtk5CS, ISwordBoundAttack {
    public State_Rosmontis_NmlAtk5CS(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new(7, 6, 10);
    public int[] indices => new[] {0};
    public override float GetUnscaledDamage(Entity to) {
        var gauge = entity.GetComponent<Gauge_Rosmontis_SwordManager>();
        return gauge.IsSwordFree(0) ? 36 : 25;
    }
    public override string GetAttackNormalSfx() {
        return "chr/rosmontis/battle/sfx/sword/dispatch/light";
    }
}

[NamedToken("Rosmontis_NmlAtk5S")]
public class State_Rosmontis_NmlAtk5S : State_Common_NmlAtk5S, ISwordBoundAttack {
    public State_Rosmontis_NmlAtk5S(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new(11, 6, 21);
    public int[] indices => new[] {0};
    public override float GetUnscaledDamage(Entity to) {
        var gauge = entity.GetComponent<Gauge_Rosmontis_SwordManager>();
        return gauge.IsSwordFree(0) ? 39 : 28;
    }
    public override string GetAttackNormalSfx() {
        return "chr/rosmontis/battle/sfx/sword/dispatch/light";
    }
}

[NamedToken("Rosmontis_NmlAtk2S")]
public class State_Rosmontis_NmlAtk2S : State_Common_NmlAtk2S, ISwordBoundAttack {
    public State_Rosmontis_NmlAtk2S(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new(10, 4, 18);
    public int[] indices => new[] {0};
    public override float GetUnscaledDamage(Entity to) {
        var gauge = entity.GetComponent<Gauge_Rosmontis_SwordManager>();
        return gauge.IsSwordFree(0) ? 36 : 26;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new(blocked ? 2f : 0f, blocked ? 0f : 5f);
    }
    public override string GetAttackNormalSfx() {
        return "chr/rosmontis/battle/sfx/sword/dispatch/light";
    }
}

[NamedToken("Rosmontis_NmlAtk5H")]
public class State_Rosmontis_NmlAtk5H : State_Common_NmlAtk5H, ISwordBoundAttack {
    public State_Rosmontis_NmlAtk5H(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new(12, 3, 28);
    public int[] indices => new[] {1};
    
    public override float GetUnscaledDamage(Entity to) {
        var gauge = entity.GetComponent<Gauge_Rosmontis_SwordManager>();
        return gauge.IsSwordFree(1) ? 53 : 38;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new(airborne ? 0 : 3, -10f);
    }
    public override string GetAttackNormalSfx() {
        return "chr/rosmontis/battle/sfx/sword/dispatch/light";
    }

    protected override void OnActive() {
        base.OnActive();
        entity.PlaySound("chr/rosmontis/battle/sfx/sword/impact/normal", .5f);
    }

    public override void OnHit(Entity target) {
        base.OnHit(target);
        if (target is PlayerCharacter opfor) {
            if (opfor.airborne) {
                opfor.frameData.AddGroundBounce(new Vector2(0, 7f));
            }
        }
    }
}

[NamedToken("Rosmontis_NmlAtk2H")]
public class State_Rosmontis_NmlAtk2H : State_Common_NmlAtk2H, ISwordBoundAttack {
    public State_Rosmontis_NmlAtk2H(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new(14, 9, 25);
    public int[] indices => new[] {1};
    
    public override float GetUnscaledDamage(Entity to) {
        var gauge = entity.GetComponent<Gauge_Rosmontis_SwordManager>();
        return gauge.IsSwordFree(1) ? 50 : 36;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new(blocked ? 5f : 2f, 12f);
    }

    public override string GetAttackNormalSfx() {
        return "chr/rosmontis/battle/sfx/sword/dispatch/light";
    }

    protected override void OnActive() {
        base.OnActive();
        // entity.PlaySound("chr/rosmontis/battle/sfx/sword/impact/normal", .5f);
    }

    public override float GetComboDecayIncreaseMultiplier(Entity to) {
        return base.GetComboDecayIncreaseMultiplier(to) * 1.5f;
    }
}

[NamedToken("Rosmontis_NmlAtk6H")]
public class State_Rosmontis_NmlAtk6H : State_Common_NmlAtk6H, ISwordBoundAttack {
    public State_Rosmontis_NmlAtk6H(Entity entity) : base(entity) { }
    public override AttackFrameData frameData => new(18, 3, 33);
    public int[] indices => new[] {0, 1};
    
    public override float GetUnscaledDamage(Entity to) {
        var gauge = entity.GetComponent<Gauge_Rosmontis_SwordManager>();
        return gauge.IsSwordFree(1) && gauge.IsSwordFree(0) ? 64 : 46;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new(blocked ? 5f : opponent.atWall ? 9f : 8f, airborne ? 7f : opponent.atWall ? 8f :-10f);
    }

    public override string GetAttackNormalSfx() {
        return "chr/rosmontis/battle/sfx/sword/dispatch/light";
    }

    protected override void OnActive() {
        base.OnActive();
        // entity.PlaySound("chr/rosmontis/battle/sfx/sword/impact/normal", .5f);protected override void OnActive() {
        player.ApplyForwardVelocity(new(4f, 0f));
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/dash_dust", CharacterFXSocketType.WORLD_UNBOUND, player.transform.position, Vector3.zero, new Vector3(player.side == EntitySide.LEFT ? 1 : -1, 1, 1));
    }

    public override float GetComboDecayIncreaseMultiplier(Entity to) {
        return base.GetComboDecayIncreaseMultiplier(to) * 1.5f;
    }

    public override void OnContact(Entity to) {
        base.OnContact(to);
        entity.PlaySound("chr/rosmontis/battle/sfx/6h/impact");
    }

    public override void OnHit(Entity target) {
        base.OnHit(target);
        if (!opponent.atWall) {
            opponent.frameData.AddGroundBounce(new Vector2(5, 5f));
            opponent.ForceSetAirborne();
        }
        
        if (target is PlayerCharacter opfor && opfor.wallDistance < 4) {
            player.ApplyForwardVelocity(new(-5, 0));
            opfor.frameData.AddWallBounce(new Vector2(2, 7f));
        }
    }
}
}
