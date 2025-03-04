using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Runtime.Gauge;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Exusiai_SklReload")]
public class State_Exusiai_SklReload : State_Common_SpecialAttack {
    private bool chambered;
    
    public State_Exusiai_SklReload(Entity entity) : base(entity) { }
    protected override string mainAnimation => "chr/SklReload";
    public override AttackFrameData frameData => new AttackFrameData() {
        startup = 20, active = 55, recovery = 16
    };

    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.DOWN, InputFrameType.PRESSED), 
        new InputFrame(InputType.BACKWARD, InputFrameType.PRESSED), 
        new InputFrame(InputType.D, InputFrameType.PRESSED),
    };

    public override Hitstate hitstate => Hitstate.COUNTER;

    protected override void OnStateBegin() {
        base.OnStateBegin();
        chambered = entity.GetComponent<Gauge_Exusiai_AmmoGauge>().chambered;
    }

    protected override void OnTick() {
        base.OnTick();
        var gauge = entity.GetComponent<Gauge_Exusiai_AmmoGauge>();
        
        if (frame == 58) {
            // reload
            gauge.Reload();
            if (chambered || gauge.displayCount == 0) {
                FastForward(75 - frame);
            }
        }

        if (frame == 62 && !chambered && gauge.displayCount > 0) {
            entity.audioManager.PlaySound("chr/exusiai/battle/sfx/gun_bolt");
        }
        
    }

    public override float GetUnscaledDamage(Entity to) {
        return 0;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return Vector2.zero;
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override int GetFreezeFrames(Entity to) {
        return 0;
    }
    public override int GetAttackLevel(Entity to) {
        return 0;
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.EXSMALL;
    }
}
}
