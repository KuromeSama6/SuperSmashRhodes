using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Chen_SklUnsheathHeavy")]
public class State_Chen_SklUnsheathHeavy : State_Common_SpecialAttack {
    public State_Chen_SklUnsheathHeavy(Entity owner) : base(owner) { }
    protected override string mainAnimation => "chen_SklUnsheathHeavy";

    public override AttackFrameData frameData => new() {
        startup = 25,
        active = 8,
        recovery = 38,
        onHit = +20,
        onBlock = -10, 
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
        return !airborne ? new(7f, 0f) : new(5f, -3f);
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
        player.PlayOwnedFx("p_chen_seq_214h_1", CharacterFXSocketType.SELF);
    }

    protected override void OnActive() {
        base.OnActive();
        owner.audioManager.PlaySound($"vo_chen_modal_{Random.Range(1, 3)}");
    }

    protected override void OnTick() {
        base.OnTick();
        if (frame == 10) {
            owner.audioManager.PlaySound("sfx_chen_SklUnsheathHeavy");
        }
    }

    public override void OnHit(Entity target) {
        base.OnHit(target);
        opponent.fxManager.PlayGameObjectFX(player.assetLibrary.GetParticle("p_chen_214h_hit1"), CharacterFXSocketType.SELF);
    }
}
}
