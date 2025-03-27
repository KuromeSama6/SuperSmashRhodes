using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Chen_SklDPC")]
public class State_Chen_SklDPC : State_Common_SpecialAttack {
    public State_Chen_SklDPC(Entity entity) : base(entity) { }
    protected override string mainAnimation => "chr/SklDPC";

    public override AttackFrameData frameData => new() {
        startup = 13,
        active = 75,
        recovery = 20
    };
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.FORWARD, InputFrameType.HELD),
        new InputFrame(InputType.DOWN, InputFrameType.PRESSED),
        new InputFrame(InputType.FORWARD, InputFrameType.PRESSED),
        new InputFrame(InputType.HS, InputFrameType.PRESSED),
    };

    public override float inputPriority => 5.5f;
    protected override int normalInputBufferLength => 15;
    public override LandingRecoveryFlag landingRecoveryFlag => LandingRecoveryFlag.UNTIL_LAND | LandingRecoveryFlag.CARRY_CANCEL_OPTIONS;

    protected override int totalHits => 3;

    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.animation.ApplyNeutralPose(true);
        stateData.shouldApplySlotNeutralPose = true;
        // player.frameData.landingFlag |= LandingRecoveryFlag.UNTIL_LAND;
    }

    public override void OnHit(Entity target) {
        base.OnHit(target);
        
        Vector3 angle = new(0f, 0f, random.Range(20, 80));
        Vector3 offset = new(random.Range(-.1f, .1f), random.Range(-.1f, .1f), 0);
        player.opponent.PlayFx("chr/chen/battle/fx/prefab/nml/slash/1", CharacterFXSocketType.SELF, offset, angle);

        if (attackStage == 2) {
            player.opponent.frameData.AddGroundBounce(new(0, driveRelease ? 12 : 7), BounceFlags.HEAVY);
        }
    }

    protected override void OnActive() {
        base.OnActive();
        entity.PlaySound("chr/chen/battle/vo/dp");
    }

    public override void OnContact(Entity to) {
        base.OnContact(to);        // stateData.physicsPushboxDisabled = true;
        if (attackStage >= 1) {
            stateData.ghostFXData = new("cb0000".HexToColor(), 0.04333334f,  20);
            SimpleCameraShakePlayer.inst.Play("chr/chen/battle/fx/camerashake", attackStage == 2 ? "dp_2" : "dp_1");
            if (attackStage == 2) BackgroundUIManager.inst.Flash(0.03f);   
        }

        entity.PlaySound("chr/chen/battle/sfx/dp/1");
        if (attackStage == 2) {
            opponent.fxManager.PlayGameObjectFX("chr/chen/battle/fx/prefab/skl_214s/hit/0", CharacterFXSocketType.SELF);
            opponent.fxManager.PlayGameObjectFX("chr/chen/battle/fx/prefab/dp/heavyhit", CharacterFXSocketType.SELF);
        }
        
    }

    public override float GetUnscaledDamage(Entity to) {
        return 30;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (blocked) return new(1, 0);
        if (attackStage == 1) {
        }
        
        return attackStage switch {
            0 => new(0.5f, 0f),
            1 => new(0.5f, 13f),
            2 => new (5f, -30f)
        };
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override int GetAttackLevel(Entity to) {
        return 3;
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.MEDIUM;
    }

    protected override void OnNotifyStage(int stage) {
        base.OnNotifyStage(stage);
        stateData.ghostFXData = new("cb0000".HexToColor(), 0.01333334f, 40, 10f);
        if (stage == 1) {
            // Debug.Log($"notify stage 1, {frame}, {routines[0].timesTicked}");
            player.ApplyForwardVelocity(new(0f, opponent.atWall && hitsRemaining < 3 ? 27.5f : 15f));
            player.airborne = true;
            
        } else if (stage == 2) {
            // Debug.Log($"notify stage 2, {frame}, {routines[0].timesTicked}");
            player.rb.linearVelocity = Vector2.zero;
            player.ApplyForwardVelocity(new(hits > 0 ? 10f : 0f, -30f));
            entity.PlaySound("chr/chen/battle/sfx/skl_214h/0", .4f);
            entity.PlaySound($"chr/chen/battle/vo/modal/{random.Range(0, 2)}");
        }
    }

    public override int GetStunFrames(Entity to, bool blocked) {
        return blocked ? 10 : base.GetStunFrames(to, blocked);
    }

    public override float GetAtWallPushbackMultiplier(Entity to) {
        return 0f;
    }
}
}
