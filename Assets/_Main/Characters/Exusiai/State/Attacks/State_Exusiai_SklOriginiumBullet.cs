using System;
using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Runtime.Gauge;
using SuperSmashRhodes.UI.Battle;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Exusiai_SklOriginiumBullet")]
public class State_Exusiai_SklOriginiumBullet : State_Common_SpecialAttack, IChargable {
    public State_Exusiai_SklOriginiumBullet(Entity entity) : base(entity) { }
    protected override string mainAnimation => "chr/SklOriginiumBullet";
    private int bulletsUsed;

    public override AttackFrameData frameData => new AttackFrameData() {
        startup = 19,
        active = 4,
        recovery = 20,
        onHit = 10, 
        onBlock = -7
    };
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.DOWN, InputFrameType.HELD), new InputFrame(InputType.BACKWARD, InputFrameType.PRESSED), new InputFrame(InputType.HS, InputFrameType.PRESSED)
    };

    public int chargeEntryFrame => 18;
    public bool mayCharge => GetCurrentInputBuffer().thisFrame.HasInput(player.side, InputType.HS, InputFrameType.HELD) && player.GetComponent<Gauge_Exusiai_AmmoGauge>().displayCount >= 7;

    protected override void OnStateBegin() {
        base.OnStateBegin();
        bulletsUsed = 0;
        
        player.audioManager.PlaySound("chr/exusiai/battle/sfx/originium/draw");
    }

    public IEnumerator ChargeRoutine() {
        int progress = 0;
        var ammoCount = player.GetComponent<Gauge_Exusiai_AmmoGauge>().displayCount;
        
        while (mayCharge) {
            progress += 1;
            // 1st charge - 10 bullets
            if (progress == (driveRelease ? 5 : 15) && ammoCount >= (driveRelease ? 5 : 10)) AddCharge(1); 
            // 2nd charge - 15 bullets
            if (progress == (driveRelease ? 30 : 75) && ammoCount >= (driveRelease ? 5 : 15)) AddCharge(1);
            yield return 1;
        }
        
        // Debug.Log("charge end");
    }

    
    protected override void OnActive() {
        base.OnActive();
        var gauge = player.GetComponent<Gauge_Exusiai_AmmoGauge>();
        if (gauge.mayFire) {
            
            BackgroundUIManager.inst.Flash(0.03f);
            player.audioManager.PlaySound($"chr/exusiai/battle/vo/modal/{Random.Range(0, 4)}");
            var bullets = chargeLevel switch {
                2 => 15,
                1 => 10,
                _ => 5
            };
            // Debug.Log(bullets);
            bulletsUsed = Math.Min(gauge.displayCount, bullets);
            player.audioManager.PlaySound("chr/exusiai/battle/sfx/214h/shot");

            for (int i = 0; i < (driveRelease ? 5 : bullets); i++) gauge.Fire(false);
            gauge.PlayMuzzleFlash();

            if (bulletsUsed >= 15) {
                player.fxManager.PlayGameObjectFXAtSocket("chr/exusiai/fx/prefab/214h/shot/lv3", "MuzzleSocket");
                SimpleCameraShakePlayer.inst.Play("chr/exusiai/battle/fx/camerashake/214h", "lv3"); 
            } else if (bulletsUsed >= 10) {
                player.fxManager.PlayGameObjectFXAtSocket("chr/exusiai/fx/prefab/214h/shot/lv2", "MuzzleSocket");
                SimpleCameraShakePlayer.inst.Play("chr/exusiai/battle/fx/camerashake/214h", "lv2"); 
            } else {
                SimpleCameraShakePlayer.inst.Play("chr/exusiai/battle/fx/camerashake/214h", "lv1");   
            }
            
            player.ApplyForwardVelocity(new(-5, 0));
            player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/super/smoke", CharacterFXSocketType.WORLD_UNBOUND, player.transform.position);
            
        } else {
            hitsRemaining = 0;
            player.audioManager.PlaySound("chr/exusiai/battle/sfx/gun_empty");
        }

    }

    
    public override void OnHit(Entity target) {
        base.OnHit(target);
        if (bulletsUsed >= 15) {
            opponent.frameData.landingFlag |= LandingRecoveryFlag.HARD_KNOCKDOWN_LAND;
            opponent.fxManager.PlayGameObjectFX("chr/exusiai/fx/prefab/214h/shot_impact/lv3", CharacterFXSocketType.WORLD);
        } else if (bulletsUsed >= 10) {
            // opponent.fxManager.PlayGameObjectFX("chr/exusiai/fx/prefab/214h/shot_impact/lv2", CharacterFXSocketType.WORLD);
        }
    }

    public override void OnContact(Entity to) {
        base.OnContact(to);
    }

    protected override void OnCharge(int newLevel) {
        base.OnCharge(newLevel);
        // Debug.Log($"charge {newLevel}");
        player.fxManager.PlayGameObjectFXAtSocket("chr/exusiai/fx/prefab/214h/charge", "MuzzleSocket");
        
        if (newLevel == 1) player.audioManager.PlaySound("chr/exusiai/battle/sfx/originium/charge_lv2");
        if (newLevel == 2) player.audioManager.PlaySound("chr/exusiai/battle/sfx/originium/charge_lv3");
    }

    public override float GetUnscaledDamage(Entity to) {
        if (bulletsUsed >= 15) return 80;
        else if (bulletsUsed >= 10) return 45;
        else return 25;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (blocked) return new(5, 5);
        if (bulletsUsed >= 15) return new Vector2(0.1f, 15f);
        if (bulletsUsed >= 10) return new Vector2(0.7f, 12f);
        return new Vector2(1f, 8f);
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override int GetFreezeFrames(Entity to) {
        if (bulletsUsed >= 15) return 10;
        return 7;
    }
    public override int GetAttackLevel(Entity to) {
        return 4;
    }

    public override int GetFrameAdvantage(Entity to, bool blocked) {
        if (!blocked) return 10;
        
        if (bulletsUsed >= 15) return +12;
        if (bulletsUsed >= 10) return +1;
        return -7;
    }

    public override CounterHitType GetCounterHitType(Entity to) {
        return bulletsUsed >= 15 ? CounterHitType.LARGE : CounterHitType.MEDIUM;
    }
}
}
