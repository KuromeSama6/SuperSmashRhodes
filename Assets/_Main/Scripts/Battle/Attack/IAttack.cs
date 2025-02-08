using UnityEngine;

namespace SuperSmashRhodes.Battle {
public interface IAttack {
    public string id { get; }
    public AttackFrameData GetFrameData(Entity to);
    public int GetCurrentFrame(Entity to);
    public AttackSpecialProperties GetSpecialProperties(Entity to);
    
    public float GetUnscaledDamage(Entity to);
    public float GetChipDamagePercentage(Entity to);
    public float GetOtgDamagePercentage(Entity to);
    public Vector2 GetPushback(Entity to, bool airborne, bool blocked);
    /**
     * If the target player is against the wall, the pushback applied
     * to the attacker is multiplied by this value.
     */
    public float GetAtWallPushbackMultiplier(Entity to);
    public Vector2 GetCarriedMomentumPercentage(Entity to);
    
    public float GetComboProration(Entity to);
    public float GetFirstHitProration(Entity to);
    public float GetComboDecay(Entity to);
    public float GetComboDecayIncreaseMultiplier(Entity to);
    public bool ShouldCountSameMove(Entity to);
    
    public AttackGuardType GetGuardType(Entity to);
    public int GetFreezeFrames(Entity to);
    public int GetAttackLevel(Entity to);

    public float GetMeterGain(Entity to, bool blocked);
    
    public bool MayHit(Entity target);
    public string GetAttackNormalSfx();
    public string GetBlockedSfx(Entity to);
    public string GetHitSfx(Entity to);
    
    public void OnContact(Entity to) {}
    public void OnHit(Entity to);
    public void OnBlock(Entity to);
}
}
