using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Runtime.Gauge;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Exusiai_SklRoll_FThrow")]
public class State_Exusiai_SklRoll_FThrow : State_Common_CommandThrow {
    public State_Exusiai_SklRoll_FThrow(Entity entity) : base(entity) {
    }
    public override EntityStateType type => EntityStateType.CHR_ATK_SPECIAL_TRIGGER;
    public override float inputPriority => 5f;
    protected override int normalInputBufferLength => 5;
    protected override string mainAnimation => "chr/SklRoll_FThrow";
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.FORWARD, InputFrameType.HELD), 
        new InputFrame(InputType.HS, InputFrameType.PRESSED)
    };

    public override AttackFrameData frameData => new AttackFrameData() {
        startup = 7,
        active = 4,
        recovery = 30
    };
    protected override string whiffAnimation => "chr/SklRoll_FThrow_W";
    protected override float distanceRequirement => 1.3f;
    protected override int animationLength => 74;

    protected override void OnStateBegin() {
        base.OnStateBegin();
        entity.PlaySound($"chr/exusiai/battle/vo/modal/{random.Range(0, 4)}");
    }

    protected override void OnThrowWhiff(PlayerCharacter other) {
        base.OnThrowWhiff(other);
        player.ApplyForwardVelocity(new(8f, 0));
    }

    protected override void OnFinalHit() {
        base.OnFinalHit();
        player.GetComponent<Gauge_Exusiai_AmmoGauge>().AddMagazine();
        player.meter.AddMeter(5);
    }
}

[NamedToken("Exusiai_SklRoll_FSlide")]
public class State_Exusiai_SklRoll_FSlide : State_Common_SpecialAttack {
    public State_Exusiai_SklRoll_FSlide(Entity entity) : base(entity) { }
    protected override string mainAnimation => "chr/SklRoll_FSlide";
    public override AttackFrameData frameData => new AttackFrameData() {
        startup = 7, active = 12, recovery = 30
    };
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.FORWARD, InputFrameType.HELD), 
        new InputFrame(InputType.DOWN, InputFrameType.HELD), 
        new InputFrame(InputType.S, InputFrameType.PRESSED)
    };

    public override EntityStateType type => EntityStateType.CHR_ATK_SPECIAL_TRIGGER;

    protected override void OnStateBegin() {
        base.OnStateBegin();
        entity.PlaySound($"chr/exusiai/battle/vo/modal/{random.Range(0, 4)}");
    }

    protected override void OnActive() {
        base.OnActive();
        player.ApplyForwardVelocity(new(11f, 0));
    }

    protected override void OnRecovery() {
        base.OnRecovery();
        // player.rb.linearVelocity = Vector2.zero;
    }

    public override void OnContact(Entity to) {
        base.OnContact(to);
        SimpleCameraShakePlayer.inst.PlayCommon("hit_medium");
    }

    public override float GetUnscaledDamage(Entity to) {
        return 32;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(1f, 6f);
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.CROUCHING;
    }
    public override int GetFreezeFrames(Entity to) {
        return 6;
    }
    public override int GetAttackLevel(Entity to) {
        return 3;
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.MEDIUM;
    }
}

[NamedToken("Exusiai_SklRoll_FEvade")]
public class State_Exusiai_SklRoll_FEvade : State_Common_UtilityMove {
    public State_Exusiai_SklRoll_FEvade(Entity entity) : base(entity) { }
    protected override string mainAnimation => "chr/SklRoll_FEvade";
    public override AttackFrameData frameData => new AttackFrameData() {
        startup = 9, active = 12, recovery = 5
    };
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.BACKWARD, InputFrameType.HELD), 
        new InputFrame(InputType.UP, InputFrameType.HELD), 
        new InputFrame(InputType.S, InputFrameType.PRESSED)
    };

    protected override float inputMeter => 1f;
    public override float inputPriority => 5f;

    public override EntityStateType type => EntityStateType.CHR_ATK_SPECIAL_TRIGGER;

    protected override void OnStateBegin() {
        base.OnStateBegin();
        entity.PlaySound($"chr/exusiai/battle/vo/modal/{random.Range(0, 4)}");
    }

    protected override void OnActive() {
        base.OnActive();
        entity.PlaySound("cmn/battle/sfx/movement/airdash");
        player.ApplyForwardVelocity(new(-6f, 10f));
        player.airborne = true;
    }

    protected override void OnRecovery() {
        base.OnRecovery();
        // player.rb.linearVelocity = Vector2.zero;
    }
}

}
