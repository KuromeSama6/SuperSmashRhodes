using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Chen_SklDPB")]
public class State_Chen_SklDPB : State_Common_DP {
    public State_Chen_SklDPB(Entity entity) : base(entity) { }
    protected override string mainAnimation => "chr/SklDPB";

    public override AttackFrameData frameData => new() {
        startup = 8,
        active = 45,
        recovery = 30,
        onHit = -3,
        onBlock = -15
    };
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.FORWARD, InputFrameType.HELD),
        new InputFrame(InputType.DOWN, InputFrameType.PRESSED),
        new InputFrame(InputType.FORWARD, InputFrameType.PRESSED),
        new InputFrame(InputType.S, InputFrameType.PRESSED),
    };

    protected override int totalHits => 3;

    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.SetCarriedStateVariable("_landingRecoveryCounterHitState", "CmnLandingRecovery", true);
        player.animation.ApplyNeutralPose(true);
        stateData.shouldApplySlotNeutralPose = true;
    }

    public override void OnHit(Entity target) {
        base.OnHit(target);
        
        Vector3 angle = new(0f, 0f, Random.Range(20, 80));
        Vector3 offset = new(Random.Range(-.1f, .1f), Random.Range(-.1f, .1f), 0);
        player.opponent.PlayFx("chr/chen/battle/fx/prefab/nml/slash/1", CharacterFXSocketType.SELF, offset, angle);
    }

    protected override void OnActive() {
        base.OnActive();
        player.audioManager.PlaySound("chr/chen/battle/vo/dp");
    }

    public override void OnContact(Entity to) {
        base.OnContact(to);        // stateData.physicsPushboxDisabled = true;
        if (attackStage >= 1) {
            stateData.ghostFXData = new("cb0000".HexToColor(), 0.04333334f,  20);
            SimpleCameraShakePlayer.inst.Play("chr/chen/battle/fx/camerashake", attackStage == 2 ? "dp_2" : "dp_1");
            opponent.fxManager.PlayGameObjectFX("chr/chen/battle/fx/prefab/skl_214s/hit/0", CharacterFXSocketType.SELF);
        }
        
        player.audioManager.PlaySound("chr/chen/battle/sfx/dp/1");
        
    }

    public override float GetUnscaledDamage(Entity to) {
        return 40;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (blocked) return new(3, 0);
        
        return attackStage switch {
            0 => new(0.5f, 0f),
            1 => new(1.5f, 13f),
        };
    }

    protected override void OnNotifyStage(int stage) {
        base.OnNotifyStage(stage);
        stateData.ghostFXData = new("cb0000".HexToColor(), 0.01333334f, 40, 10f);
        if (stage == 1) {
            player.ApplyForwardVelocity(new(.5f, opponent.atWall && hitsRemaining < 2 ? 20f : 10f));
            player.airborne = true;
            entity.audioManager.PlaySound("chr/chen/battle/sfx/skl_214h/0", .4f);   
            
        } else if (stage == 2) {
            stateData.ghostFXData = null;
            player.animation.AddUnmanagedAnimation("std/jump_down", false, .1f);
        }
    }

    protected override void OnStateEnd(EntityState nextState) {
        base.OnStateEnd(nextState);
    }

    public override int GetStunFrames(Entity to, bool blocked) {
        return blocked ? 10 : base.GetStunFrames(to, blocked);
    }

    public override float GetAtWallPushbackMultiplier(Entity to) {
        return 0f;
    }
}
}
