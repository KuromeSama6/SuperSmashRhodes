using UnityEngine;

namespace SuperSmashRhodes.Battle {
public interface IAttack {
    string id { get; }
    AttackFrameData GetFrameData(Entity to);
    int GetStunFrames(Entity to, bool blocked);
    AttackSpecialProperties GetSpecialProperties(Entity to);
    DamageSpecialProperties GetDamageSpecialProperties(Entity to);
    
    float GetUnscaledDamage(Entity to);
    float GetChipDamagePercentage(Entity to);
    float GetOtgDamagePercentage(Entity to);
    Vector2 GetPushback(Entity to, bool airborne, bool blocked);
    /**
     * If the target player is against the wall, the pushback applied
     * to the attacker is multiplied by this value.
     */
    float GetAtWallPushbackMultiplier(Entity to);
    Vector2 GetCarriedMomentumPercentage(Entity to);
    
    float GetComboProration(Entity to);
    float GetFirstHitProration(Entity to);
    float GetComboDecay(Entity to);
    float GetComboDecayIncreaseMultiplier(Entity to);
    float GetMinimumDamagePercentage(Entity to);
    bool ShouldCountSameMove(Entity to);
    
    AttackGuardType GetGuardType(Entity to);
    int GetFreezeFrames(Entity to);
    int GetAttackLevel(Entity to);

    float GetMeterGain(Entity to, bool blocked);
    
    bool MayHit(Entity target);
    string GetAttackNormalSfx();
    string GetBlockedSfx(Entity to);
    string GetHitSfx(Entity to);
    
    void OnContact(Entity to) {}
    void OnHit(Entity to);
    void OnBlock(Entity to);
}
}
