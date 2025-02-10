using System.Collections;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.Tokens {
public class Token_Exusiai_ApplePie : Token {
    public GameObject explosionFxSmall, explosionFxLarge;
    public SimpleCameraShakeData shakeSmall, shakeLarge;
    public ExplicitBoundingBox boundingBox;
    
    public override TokenFlag flags => TokenFlag.DESTROY_ON_OWNER_DAMAGE;
    public SpriteRenderer spriteRenderer { get; private set; }
    
    protected override void Start() {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        AttachToBone("F_L_Hand");
        boundingBox.boxEnabled = false;
    }

    protected override EntityState GetDefaultState() {
        return new State_Token_Exusiai_ApplePie_Main(this);
    }

    protected override IAttack OnOutboundHit(Entity victim, EntityBBInteractionData data) {
        return activeState as IAttack;
    }
}

[NamedToken("Token_Exusiai_ApplePie_Main")]
public class State_Token_Exusiai_ApplePie_Main : TokenState, ITokenSingleAttack {
    private Token_Exusiai_ApplePie applePie;
    private float flashCounter;
    private int fuse;
    private int fuseLength => 180;

    private float flashInterval => Mathf.Lerp(0.05f, 0.7f, fuse / (float)fuseLength);
    private Vector3 baseScale;
    public bool isLargeExplosion { get; set; }
    
    public State_Token_Exusiai_ApplePie_Main(Entity entity) : base(entity) {
        applePie = (Token_Exusiai_ApplePie)entity;
        baseScale = entity.transform.localScale;
    }
    
    protected override void OnStateBegin() {
        base.OnStateBegin();
        flashCounter = 0;
        fuse = fuseLength;
        isLargeExplosion = true;
    }
    
    public override IEnumerator MainRoutine() {
        while (fuse > 0) {
            --fuse;
            flashCounter += Time.fixedDeltaTime;
            if (flashCounter >= flashInterval) {
                flashCounter = 0;
                entity.StartCoroutine(Flash());
                entity.transform.localScale = baseScale * 1.5f;
                // entity.rb.AddForce(new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 3f)), ForceMode2D.Impulse);
            }
            yield return 1;
        }
        
        // yield return Detonate(true);
        // entity.DestroySummon(entity);
        
        // Debug.Log("Detonate1");
        var fx = isLargeExplosion ? applePie.explosionFxLarge : applePie.explosionFxSmall;
        var sfx = $"chr/exusiai/battle/sfx/apple_pie_explode{Random.Range(1, 3)}";
        var shake = isLargeExplosion ? applePie.shakeLarge : applePie.shakeSmall;
        entity.rb.mass = 0;
        entity.rb.gravityScale = 0;
        entity.rb.linearVelocity = Vector2.zero;

        ((BoxCollider2D)applePie.boundingBox.box).size *= isLargeExplosion ? 1f : 0.5f;
        
        var explosion = Object.Instantiate(fx, entity.transform);
        explosion.transform.localScale *= 5f;
        explosion.transform.localPosition += new Vector3(0, 1f, 0);
        explosion.transform.localEulerAngles += new Vector3(0, 90f, 0);
        var fxPlayer = explosion.GetComponent<ParticleFXPlayer>() ?? explosion.AddComponent<ParticleFXPlayer>(); 
        entity.owner.audioManager.PlaySound(sfx);
        SimpleCameraShakePlayer.inst.Play(shake);
        applePie.spriteRenderer.color = Color.clear;

        applePie.boundingBox.boxEnabled = true;
        
        int framesElapsed = 0;
        while (fxPlayer) {
            applePie.spriteRenderer.color = Color.clear;
            ++framesElapsed;
            if (framesElapsed > 5f) {
                applePie.boundingBox.boxEnabled = false;
            }
            yield return 1;
        }
    }

    private IEnumerator Flash() {
        applePie.spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        applePie.spriteRenderer.color = Color.white;
    }
    
    public void DetonateImmediate() {
        isLargeExplosion = false;
        fuse = 0;
    }
    
    protected override void OnTick() {
        base.OnTick();
        entity.transform.localScale = Vector3.Lerp(entity.transform.localScale, baseScale, Time.fixedDeltaTime * 10f);
    }
    public int GetCurrentFrame(Entity to) {
        return frame;
    }
    public AttackSpecialProperties GetSpecialProperties(Entity to) {
        return AttackSpecialProperties.NONE;
    }
    public float GetUnscaledDamage(Entity to) {
        // Debug.Log(isLargeExplosion ? 65 : 40);
        return isLargeExplosion ? 65 : 40;
    }
    public Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (blocked) return isLargeExplosion ? new Vector2(5f, 0) : new Vector2(3f, 0);
        return isLargeExplosion ? new Vector2(5f, 15f) : new Vector2(1.5f, 10f);
    }
    public float GetComboProration(Entity to) {
        return .9f;
    }
    public float GetFirstHitProration(Entity to) {
        return 1f;
    }
    public AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public int GetFreezeFrames(Entity to) {
        return isLargeExplosion ? 15 : 8;
    }
    public int GetAttackLevel(Entity to) {
        return isLargeExplosion ? 3 : 2;
    }
    public bool MayHit(Entity target) {
        return true;
    }
    public void OnHit(Entity to) {
        applePie.boundingBox.boxEnabled = false;
    }
    
    public void OnBlock(Entity to) {
        applePie.boundingBox.boxEnabled = false;
        
    }
    public int GetStunFrames(Entity to, bool blocked) {
        return 10;
    }
}
}