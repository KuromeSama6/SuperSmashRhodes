using UnityEngine;

namespace SuperSmashRhodes.Battle {
public interface ITokenSingleAttack : IAttack {
    AttackFrameData IAttack.GetFrameData(Entity to) {
        return new AttackFrameData();
    }
    DamageSpecialProperties IAttack.GetDamageSpecialProperties(Entity to) {
        return DamageSpecialProperties.NONE;
    }
    bool IAttack.ShouldCountSameMove(Entity to) {
        return false;
    }
    
    float IAttack.GetChipDamagePercentage(Entity to) {
        return .12f;
    }
    float IAttack.GetOtgDamagePercentage(Entity to) {
        return .6f;
    }

    float IAttack.GetAtWallPushbackMultiplier(Entity to) {
        return 1f;
    }
    Vector2 IAttack.GetCarriedMomentumPercentage(Entity to) {
        return new Vector2(.5f, .2f);
    }

    float IAttack.GetComboDecayIncreaseMultiplier(Entity to) {
        return 1f;
    }

    float IAttack.GetMinimumDamagePercentage(Entity to) {
        return 0f;
    }

    string IAttack.GetAttackNormalSfx() {
        return null;
    }
    string IAttack.GetBlockedSfx(Entity to) {
        return "cmn/battle/sfx/block/2";
    }
    float IAttack.GetComboDecay(Entity to) {
        return 1f;
    }
    string IAttack.GetHitSfx(Entity to) {
        return "cmn/battle/sfx/hit/1";
    }
    float IAttack.GetMeterGain(Entity to, bool blocked) {
        var attackLevel = GetAttackLevel(to);
        return (attackLevel + 1) * 1.5f * (blocked ? 1f : 2f);
    }
}
}
