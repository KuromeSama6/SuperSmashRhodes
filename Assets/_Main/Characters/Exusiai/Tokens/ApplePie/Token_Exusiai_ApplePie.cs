using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.Tokens {
public class Token_Exusiai_ApplePie : Token {
    public GameObject explosionFxSmall, explosionFxLarge;
    public SimpleCameraShakeData shakeSmall, shakeLarge;
    public ExplicitBoundingBox boundingBox;
    
    public override TokenFlag flags => TokenFlag.DESTROY_ON_OWNER_DAMAGE;
    public SpriteRenderer spriteRenderer { get; private set; }

    public override void Init() {
        base.Init();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (_serializedAttached) AttachToBone("F_L_Hand");
        boundingBox.boxEnabled = false;
    }

    public override EntityState GetDefaultState() {
        return states["Token_Exusiai_ApplePie_Main"];
    }

    protected override IAttack OnOutboundHit(Entity victim, EntityBBInteractionData data) {
        return activeState as IAttack;
    }
}

[NamedToken("Token_Exusiai_ApplePie_Main")]
public class State_Token_Exusiai_ApplePie_Main : TokenState {
    private Token_Exusiai_ApplePie applePie;
    private float flashCounter;
    public int fuse => fuseLength - frame;
    private int fuseLength => 180;

    private float flashInterval => Mathf.Lerp(0.05f, 0.7f, fuse / (float)fuseLength);
    public override EntityStateType type => EntityStateType.ENT_TOKEN;
    private Vector3 baseScale;
    public bool isLargeExplosion { get; set; }

    public State_Token_Exusiai_ApplePie_Main(Entity entity) : base(entity) {
        // Debug.Log($"new piestate {GetHashCode()} {entity.states.ContainsValue(this)}");
        applePie = (Token_Exusiai_ApplePie)entity;
        baseScale = entity.transform.localScale;
    }
    
    protected override void OnTick() {
        base.OnTick();
        entity.transform.localScale = Vector3.Lerp(entity.transform.localScale, baseScale, Time.fixedDeltaTime * 10f);
        flashCounter += Time.fixedDeltaTime;
        if (flashCounter >= flashInterval && entity) {
            flashCounter = 0;
            entity.StartCoroutine(Flash());
            entity.transform.localScale = baseScale * 1.5f;
            // entity.rb.AddForce(new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 3f)), ForceMode2D.Impulse);
        }
    }
    
    protected override void OnStateBegin() {
        base.OnStateBegin();
        flashCounter = 0;
        isLargeExplosion = true;
        entity.rb.simulated = false;
        // Debug.Log("state begin");
    }
    
    public override EntityStateSubroutine BeginMainSubroutine() {
        EnsureEntity();
        return ctx => ctx.Exit(fuseLength);
    }

    protected override void OnStateEndComplete(EntityState nextState) {
        base.OnStateEndComplete(nextState);
        Detonate();
    }

    private IEnumerator Flash() {
        applePie.spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        applePie.spriteRenderer.color = Color.white;
    }
    
    public void DetonateImmediate() {
        isLargeExplosion = false;
        Detonate();
    }

    private void Detonate() {
        entity.SetCarriedStateVariable("isLargeExplosion", null, isLargeExplosion);
        CancelInto("Token_Exusiai_ApplePie_Attack");
    }
}

[NamedToken("Token_Exusiai_ApplePie_Attack")]
public class State_Token_Exusiai_ApplePie_Attack : TokenAttackStateBase {
    private bool isLargeExplosion;
    private Token_Exusiai_ApplePie applePie;
    private ParticleFXPlayer fxPlayer;
    
    public State_Token_Exusiai_ApplePie_Attack(Entity entity) : base(entity) {
        applePie = (Token_Exusiai_ApplePie)entity;
    }
    protected override string mainAnimation { get; } = null;
    public override AttackFrameData frameData => new AttackFrameData() {
        startup = 0, active = 5, recovery = 1
    };

    protected override bool mayDestroy => true;

    protected override void OnStateBegin() {
        base.OnStateBegin();
        isLargeExplosion = stateData.GetCarriedVariable<bool>("isLargeExplosion");
        Object.Destroy(fxPlayer);
        fxPlayer = null;
    }

    protected override void OnActive() {
        base.OnActive();
        var fx = isLargeExplosion ? applePie.explosionFxLarge : applePie.explosionFxSmall;
        var sfx = $"chr/exusiai/battle/sfx/apple_pie_explode{Random.Range(1, 3)}";
        var shake = isLargeExplosion ? applePie.shakeLarge : applePie.shakeSmall;
        entity.rb.simulated = false;

        ((BoxCollider2D)applePie.boundingBox.box).size *= isLargeExplosion ? 1f : 0.5f;

        var explosion = Object.Instantiate(fx);
        explosion.transform.position = entity.transform.position;
        explosion.transform.localScale *= 1f;
        explosion.transform.localPosition += new Vector3(0, 1f, 0);
        explosion.transform.localEulerAngles += new Vector3(0, 90f, 0);
        fxPlayer = explosion.GetComponent<ParticleFXPlayer>() ?? explosion.AddComponent<ParticleFXPlayer>(); 
        entity.owner.audioManager.PlaySound(sfx);
        SimpleCameraShakePlayer.inst.Play(shake);
        applePie.spriteRenderer.color = Color.clear;

        applePie.boundingBox.boxEnabled = true;
    }

    protected override void OnTick() {
        base.OnTick();
        applePie.spriteRenderer.color = Color.clear;
    }

    public override float GetUnscaledDamage(Entity to) {
        return isLargeExplosion ? 65 : 40;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (blocked) return isLargeExplosion ? new Vector2(5f, 0) : new Vector2(3f, 0);
        return isLargeExplosion ? new Vector2(5f, 15f) : new Vector2(0.5f, 10f);
    }
    public override float GetComboProration(Entity to) {
        return .9f;
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override int GetFreezeFrames(Entity to) {
        return isLargeExplosion ? 15 : 8;
    }
    public override int GetAttackLevel(Entity to) {
        return isLargeExplosion ? 3 : 2;
    }
}
}