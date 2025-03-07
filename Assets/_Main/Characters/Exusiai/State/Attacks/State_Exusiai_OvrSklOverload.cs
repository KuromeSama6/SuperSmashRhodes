using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Animation;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Runtime.Gauge;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Exusiai_OvrSklOverload")]
public class State_Exusiai_OvrSklOverload : State_Common_OverdriveAttack {
    protected override string mainAnimation => "chr/OvrSklOverload";
    public override AttackFrameData frameData => new() {
        startup = 20,
        active = 10,
        recovery = 9
    };
    // 632146D
    protected override InputFrame[] requiredInput => new InputFrame[] {
        new(InputType.FORWARD, InputFrameType.HELD),
        new(InputType.DOWN, InputFrameType.PRESSED),
        new(InputType.BACKWARD, InputFrameType.PRESSED),
        new(InputType.FORWARD, InputFrameType.PRESSED),
        new(InputType.D, InputFrameType.PRESSED),
    };
    protected override int framesBeforeSuperfreeze => 13;
    protected override int farHitSkipFrame => 360;
    protected override int farHitActiveFrames => 2;
    protected override float cinematicHitDistance => 4f;
    protected override float cinematicTotalLength => 381; 
    public override StateIndicatorFlag stateIndicator => base.stateIndicator | (cinematicHit ? StateIndicatorFlag.INVINCIBLE : StateIndicatorFlag.NONE);

    public State_Exusiai_OvrSklOverload(Entity entity) : base(entity) { }

    public override void OnSocketRelease(AnimationEventData data) {
        base.OnSocketRelease(data);
        // add velocity
        
    }

    public override float GetUnscaledDamage(Entity to) {
        return 60;
    }

    protected override void OnSuperHit(bool blocked, bool cinematic) {
        base.OnSuperHit(blocked, cinematic);
        entity.PlaySound($"chr/exusiai/battle/vo/modal/{random.Range(0, 4)}");
        if (cinematic) {
            opponent.stateFlags |= CharacterStateFlag.NO_CAMERA_WEIGHT;
            player.stateFlags |= CharacterStateFlag.CAMERA_FOLLOWS_BONE;

            var gauge = player.GetComponent<Gauge_Exusiai_AmmoGauge>();
            if (gauge.chambered) gauge.currentMagazine.ammo = 30;

            player.CallLaterCoroutine(1.3f, () => {
                entity.PlaySound($"chr/exusiai/battle/vo/632146d/{random.Range(0, 2)}");
            });
            
        }
    }

    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(blocked ? 1.5f : 10, 5);
    }
    
    protected override void OnWhiff() {
        base.OnWhiff();
        entity.PlaySound($"chr/exusiai/battle/vo/modal/{random.Range(0, 4)}");
    }

    public override string GetAttackNormalSfx() {
        return "chr/exusiai/battle/sfx/632146d/shot";
    }

    public override void OnContact(Entity to) {
        base.OnContact(to);
        
        BackgroundUIManager.inst.Flash(0.05f);
        SimpleCameraShakePlayer.inst.Play("chr/exusiai/battle/fx/camerashake/632146d", "shot"); 
    }

    public override void OnHit(Entity target) {
        base.OnHit(target);
        if (cinematicHit) {
            opponent.fxManager.PlayGameObjectFX("chr/exusiai/fx/prefab/632146d/impact_star", CharacterFXSocketType.WORLD_UNBOUND, opponent.transform.position);
        }
    }

    [AnimationEventHandler("Overload_FireBlast")]
    public virtual void OnFireBlast(AnimationEventData data) {
        if (!cinematicHit) return;
        entity.PlaySound("chr/exusiai/battle/sfx/632146d/blast");
        BackgroundUIManager.inst.Flash(0.05f);
        
        SimpleCameraShakePlayer.inst.Play("chr/exusiai/battle/fx/camerashake/632146d", "blast");
        
        if (cinematicHit) {
            opponent.fxManager.PlayGameObjectFX("chr/exusiai/fx/prefab/632146d/explosion", CharacterFXSocketType.WORLD_UNBOUND, opponent.transform.position);
            player.opponent.ApplyDamage(38f, CreateAttackData(opponent), DamageProperties.SKIP_REGISTER);
        }
        
        TimeManager.inst.globalFreezeFrames = 15;
        opponent.ApplyCarriedPushback(new Vector2(10, 8), Vector2.zero, 0f);
    }
    
    [AnimationEventHandler("FireWeaponSfx")]
    public virtual void OnFireWeaponSfx(AnimationEventData args) {
        player.GetComponent<Gauge_Exusiai_AmmoGauge>().PlayMuzzleFlash();
        
        opponent.fxManager.PlayGameObjectFX("chr/exusiai/battle/fx/prefab/632146d/hit_sparks", CharacterFXSocketType.WORLD_UNBOUND, opponent.transform.position, new Vector3(0, 0, random.Range(0, 360)));
        
        opponent.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/attack_land/0", CharacterFXSocketType.WORLD_UNBOUND, opponent.transform.position, new Vector3(0, 0, random.Range(0, 360)));
        
        SimpleCameraShakePlayer.inst.Play("chr/exusiai/battle/fx/camerashake/632146d", "shot");
        
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.MEDIUM;
    }
}
}
