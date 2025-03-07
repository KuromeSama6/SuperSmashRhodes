using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Chen_SklUnsheathLight")]
public class State_Chen_SklUnsheathLight : State_Common_SpecialAttack {
    public State_Chen_SklUnsheathLight(Entity entity) : base(entity) { }
    protected override string mainAnimation => "chr/SklUnsheathLight";

    public override AttackFrameData frameData => new() {
        startup = 15,
        active = 3,
        recovery = 24, 
    };
    
    // 214S
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.DOWN, InputFrameType.PRESSED),
        new InputFrame(InputType.BACKWARD, InputFrameType.PRESSED),
        new InputFrame(InputType.S, InputFrameType.PRESSED),
    };

    public override AttackSpecialProperties GetSpecialProperties(Entity to) {
        return AttackSpecialProperties.SOFT_KNOCKDOWN;
    }

    public override float GetUnscaledDamage(Entity to) {
        return 40f;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (blocked) return new(3.5f, 0f);
        return !airborne ? new(4.5f, 0f) : new(5f, -3f);
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override int GetFreezeFrames(Entity to) {
        return 10;
    }
    public override int GetAttackLevel(Entity to) {
        return 3;
    }
    public override string GetAttackNormalSfx() {
        return null;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.PlayOwnedFx("prefab/skl_214s/0", CharacterFXSocketType.SELF);
    }

    protected override void OnActive() {
        base.OnActive();
        entity.PlaySound($"chr/chen/battle/vo/modal/{random.Range(0, 2)}");
        AssetManager.Get<GameObject>("chr/chen/battle/fx/prefab/skl_214s/hit/0", go => {
            player.fxManager.PlayGameObjectFX(go, CharacterFXSocketType.WORLD, new(1.5f, -.2f, 0));
        }); 
    }

    protected override void OnTick() {
        base.OnTick();
        if (frame == 10) {
            entity.PlaySound("chr/chen/battle/sfx/skl_214h/0", .4f);
        }
    }

    public override void OnHit(Entity target) {
        base.OnHit(target);
        
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.MEDIUM;
    }
}
}
