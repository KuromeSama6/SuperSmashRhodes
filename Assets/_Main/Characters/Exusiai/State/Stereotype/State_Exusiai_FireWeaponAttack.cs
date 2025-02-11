using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Runtime.Gauge;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Exusiai_FireWeaponAttack : CharacterAttackStateBase {
    protected Gauge_Exusiai_AmmoGauge gauge { get; }
    protected override float inputMeter => 0f;

    protected State_Exusiai_FireWeaponAttack(Entity entity) : base(entity) {
        gauge = entity.GetComponent<Gauge_Exusiai_AmmoGauge>();
    }
    
    public override float GetOtgDamagePercentage(Entity to) {
        return 0.6f;
    }
    public override float GetComboProration(Entity to) {
        return 0.75f;
    }
    public override float GetFirstHitProration(Entity to) {
        return 1f;
    }
    public override int GetFreezeFrames(Entity to) {
        return 1;
    }
    public override int GetAttackLevel(Entity to) {
        return 3;
    }
    public override bool ShouldCountSameMove(Entity to) {
        return false;
    }
}
}
