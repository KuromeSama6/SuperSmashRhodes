using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Runtime.State;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.Character {
[NamedToken("Rosmontis_SklCrescentSaber")]
public class State_Rosmontis_SklCrescentSaber : State_Common_SpecialAttack, ISwordBoundAttack {
    public State_Rosmontis_SklCrescentSaber(Entity entity) : base(entity) { }
    protected override AttackAirOkType airOk => AttackAirOkType.AIR;
    public override LandingRecoveryFlag landingRecoveryFlag => LandingRecoveryFlag.UNTIL_LAND;
    protected override string mainAnimation => "chr/SklCrescentSaber";
    public override AttackFrameData frameData => new(16, 71, 9);
    public int[] indices => new[] { 1 };
    protected override int totalHits => 2;

    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.DOWN, InputFrameType.PRESSED), 
        new InputFrame(InputType.BACKWARD, InputFrameType.PRESSED), 
        new InputFrame(InputType.HS, InputFrameType.PRESSED),
    };

    public override float GetUnscaledDamage(Entity to) {
        return attackStage == 0 ? 33 : 50;
    }

    protected override void OnStartup() {
        base.OnStartup();
        player.rb.linearVelocity = Vector2.zero;
        stateData.gravityScale = 0;

        entity.PlaySound("chr/rosmontis/battle/sfx/j214h/swing");
    }

    protected override void OnActive() {
        base.OnActive();
    }

    protected override void OnNotifyStage(int stage) {
        base.OnNotifyStage(stage);
        entity.PlaySound("chr/rosmontis/battle/sfx/j214h/swing");
    }

    public override void OnHit(Entity target) {
        base.OnHit(target);
        if (attackStage == 1) {
            if (opponent.airborne) {
                opponent.frameData.AddGroundBounce(new(3, 5), BounceFlags.HEAVY);
                opponent.frameData.landingFlag |= LandingRecoveryFlag.HARD_KNOCKDOWN_LAND;
            }
        }
    }

    public override void OnContact(Entity to) {
        base.OnContact(to);
        if (attackStage == 1) entity.PlaySound("chr/rosmontis/battle/sfx/j214h/hitfinal");
    }

    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return attackStage == 0 ? new Vector2(0.5f, 10f) : new Vector2(5f, -30f);
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override int GetAttackLevel(Entity to) {
        return attackStage == 0 ? 3 : 4;
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return attackStage == 0 ? CounterHitType.SMALL : CounterHitType.MEDIUM;
    }

}
}
