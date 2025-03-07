using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnBurst")]
public class State_CmnBurst : CharacterAttackStateBase {
    public State_CmnBurst(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_SYSTEMSPECIAL;
    public override float inputPriority => 10;
    protected override string mainAnimation => "cmn/NmlSysBurst";
    public override AttackFrameData frameData => new AttackFrameData() {
        startup = 5, active = 11, recovery = 39
    };

    protected override EntityStateType commonCancelOptions => EntityStateType.NONE;
    protected override InputFrame[] requiredInput => null;
    protected override int normalInputBufferLength => 3;
    protected override float inputMeter => 0;
    public override AttackType invincibility => phase == AttackPhase.RECOVERY ? AttackType.NONE : AttackType.FULL;
    public override bool mayEnterState => player.activeState is State_Common_Stun && player.burst.canBurst;
    public override StateIndicatorFlag stateIndicator => StateIndicatorFlag.REVERSAL | (phase == AttackPhase.RECOVERY ? StateIndicatorFlag.NONE : StateIndicatorFlag.INVINCIBLE);

    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.rb.linearVelocity = Vector2.zero;
        // player.rb.AddForce(new Vector2(0, 15), ForceMode2D.Impulse);
        
        player.burst.AddDeltaTotal(-450, 60);
        player.burst.burstAvailable = false;
        player.burst.burstUsed = true;

        stateData.backgroundUIData = new(1, .95f, 16f, BackgroundType.NONE, Color.white, 0);
        stateData.gravityScale = 0;
    }

    protected override void OnActive() {
        base.OnActive();
        RemoveCancelOption("CmnWhiteForceReset");
        var pos = player.transform.position;
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/burst", CharacterFXSocketType.WORLD_UNBOUND, pos);
        entity.PlaySound("cmn/battle/sfx/burst");
        stateData.gravityScale = 0;
        player.rb.linearVelocity = Vector2.zero;
        // player.airborne = true;
    }

    protected override void OnRecovery() {
        base.OnRecovery();
        stateData.gravityScale = 1;
        
        stateData.backgroundUIData = BackgroundUIData.DEFAULT;
    }

    public override bool IsInputValid(InputBuffer buffer) {
        var thisFrame = buffer.thisFrame;
        return thisFrame.HasInput(player.side, InputType.S, InputFrameType.PRESSED)
            && thisFrame.HasInput(player.side, InputType.HS, InputFrameType.PRESSED)
            && thisFrame.HasInput(player.side, InputType.D, InputFrameType.PRESSED)
            && thisFrame.HasInput(player.side, InputType.P, InputFrameType.PRESSED);
    }

    public override float GetUnscaledDamage(Entity to) {
        return 0;
    }
    public override float GetChipDamagePercentage(Entity to) {
        return 0;
    }
    public override float GetOtgDamagePercentage(Entity to) {
        return 0;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (blocked) {
            return airborne ? new Vector2(5, 3) : new Vector2(3, 0);
        }
        return new Vector2(10, 10);
    }
    public override float GetComboProration(Entity to) {
        return 0;
    }
    public override float GetFirstHitProration(Entity to) {
        return 0;
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override int GetFreezeFrames(Entity to) {
        return 15;
    }
    public override int GetAttackLevel(Entity to) {
        return 3;
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.EXSMALL;
    }
    public override AttackSpecialProperties GetSpecialProperties(Entity to) {
        return AttackSpecialProperties.IGNORE_INVINCIBILITY | AttackSpecialProperties.HARD_KNOCKDOWN | AttackSpecialProperties.FORCE_LAUNCH;
    }
    public override string GetAttackNormalSfx() {
        return null;
    }
    
    public override void OnHit(Entity target) {
        base.OnHit(target);
        player.burst.AddDeltaTotal(150, 120);
    }

    public override float GetAtWallPushbackMultiplier(Entity to) {
        return 0;
    }

    public override void OnBlock(Entity target) {
        base.OnBlock(target);
    }
}
}
