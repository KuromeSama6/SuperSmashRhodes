using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Chen_SklUnsheathHeavy")]
public class State_Chen_SklUnsheathHeavy : State_Common_SpecialAttack {
    public State_Chen_SklUnsheathHeavy(Entity entity) : base(entity) { }
    protected override string mainAnimation => "chr/SklUnsheathHeavy";

    public override AttackFrameData frameData => new() {
        startup = 25,
        active = 8,
        recovery = 38, 
    };
    
    // 214H
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.DOWN, InputFrameType.PRESSED),
        new InputFrame(InputType.BACKWARD, InputFrameType.PRESSED),
        new InputFrame(InputType.HS, InputFrameType.PRESSED),
    };

    public override AttackSpecialProperties GetSpecialProperties(Entity to) {
        return AttackSpecialProperties.HARD_KNOCKDOWN;
    }

    public override float GetUnscaledDamage(Entity to) {
        return 66f;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (blocked) return new(5f, 0f);
        return !airborne ? Vector2.zero : new(0, -3f);
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override int GetFreezeFrames(Entity to) {
        return 10;
    }
    public override int GetAttackLevel(Entity to) {
        return 4;
    }
    public override string GetAttackNormalSfx() {
        return null;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.PlayOwnedFx("prefab/skl_214h/0", CharacterFXSocketType.SELF);
    }

    protected override void OnActive() {
        base.OnActive();
        entity.PlaySound($"chr/chen/battle/vo/modal/{random.Range(0, 3)}");
    }

    protected override void OnTick() {
        base.OnTick();
        if (frame == 10) {
            entity.PlaySound("chr/chen/battle/sfx/skl_214h/0");
        }
    }

    public override void OnHit(Entity target) {
        base.OnHit(target);
        AssetManager.Get<GameObject>("chr/chen/battle/fx/prefab/skl_214h/hit/0", go => {
            opponent.fxManager.PlayGameObjectFX(go, CharacterFXSocketType.SELF);
        });
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.LARGE;
    }
}
}
