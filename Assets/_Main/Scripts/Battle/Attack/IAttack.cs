using UnityEngine;

namespace SuperSmashRhodes.Battle {
public interface IAttack {
    public AttackFrameData GetFrameData(Entity to);
    public int GetCurrentFrame(Entity to);
    
    public float GetUnscaledDamage(Entity to);
    public float GetChipDamagePercentage(Entity to);
    public float GetOtgDamagePercentage(Entity to);
    public Vector2 GetPushback(Entity to, bool airborne);
    
    public float GetComboProration(Entity to);
    public float GetFirstHitProration(Entity to);
    public AttackGuardType GetGuardType(Entity to);
    public int GetFreezeFrames(Entity to);
    public int GetAttackLevel(Entity to);

    public bool MayHit(Entity target);
    public string GetAttackNormalSfx();
    public string GetBlockedSfx(Entity to);
    public string GetHitSfx(Entity to);
    
    public void OnContact(Entity to) {}
    public void OnHit(Entity to);
    public void OnBlock(Entity to);
}
}
