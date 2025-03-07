using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Animation;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Chen_Skl7HolyScriptureBreak")]
public class State_Chen_Skl7HolyScriptureBreak : State_Common_SpecialAttack {
    public State_Chen_Skl7HolyScriptureBreak(Entity entity) : base(entity) { }
    protected override string mainAnimation => "chr/Skl7HolyScriptureBreak";
    public override AttackFrameData frameData => new() {
        startup = 41, active = 21, recovery = 26
    };
    protected override InputFrame[] requiredInput => new InputFrame[] {
        new(InputType.BACKWARD, InputFrameType.HELD),
        new(InputType.DOWN, InputFrameType.PRESSED),
        new(InputType.FORWARD, InputFrameType.PRESSED),
        new(InputType.D, InputFrameType.PRESSED),
    };

    public override StateIndicatorFlag stateIndicator => StateIndicatorFlag.NONE;

    private bool armor;

    protected override void OnStartup() {
        base.OnStartup();
        stateData.shouldApplySlotNeutralPose = true;
        entity.PlaySound($"chr/chen/battle/vo/41236d");
        
        player.ApplyForwardVelocity(new(-5, 5));
        armor = false;
    }

    protected override void OnActive() {
        base.OnActive();
        if (GetCurrentInputBuffer().thisFrame.HasInput(player.side, InputType.D, InputFrameType.HELD)) {
            CancelInto("CmnNeutral");
            return;
        }
        
        stateData.ghostFXData = new("cb0000".HexToColor(), 0.02333334f, driveRelease ? 40 : 20, 2.5f);
        // stateData.physicsPushboxDisabled = true;
        entity.PlaySound("chr/chen/battle/sfx/drive/p1");
        entity.PlaySound("chr/chen/battle/sfx/skl_214h/0", .4f);  
        
        entity.PlaySound($"chr/chen/battle/vo/modal/{random.Range(0, 2)}");
        player.fxManager.PlayGameObjectFX("chr/chen/battle/fx/prefab/drive_dash_smoke", CharacterFXSocketType.WORLD_UNBOUND, player.transform.position, Vector3.zero, new(player.side == EntitySide.LEFT ? -1 : 1, 1, 1));
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/land/medium", CharacterFXSocketType.WORLD_UNBOUND, player.transform.position);
        player.ApplyForwardVelocity(new Vector2(25, 0));
    }

    protected override void OnRecovery() {
        base.OnRecovery();
        stateData.ghostFXData = null;
        player.ApplyGroundedFrictionImmediate();
        stateData.physicsPushboxDisabled = false;
        
    }

    public override void OnContact(Entity to) {
        base.OnContact(to);
        BackgroundUIManager.inst.Flash(0.05f);
        opponent.fxManager.PlayGameObjectFX("chr/chen/battle/fx/prefab/skl_214s/hit/0", CharacterFXSocketType.SELF);
        SimpleCameraShakePlayer.inst.Play("chr/chen/battle/fx/camerashake", "5d");
        player.rb.linearVelocity = Vector2.zero;
    }

    public override void OnHit(Entity target) {
        base.OnHit(target);
        stateData.midscreenWallDistanceModifier -= 5;
        
        opponent.frameData.AddWallBounce(new Vector2(4f, 10));
    }

    public override float GetUnscaledDamage(Entity to) {
        return 65;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        if (blocked) return new(6f, 0);
        return new(20, 12);
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override int GetFreezeFrames(Entity to) {
        return 10;
    }
    public override int GetAttackLevel(Entity to) {
        return 4;
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.LARGE;
    }

    public override float GetAtWallPushbackMultiplier(Entity to) {
        return 0f;
    }

    public override InboundHitModifier OnHitByOther(AttackData attackData) {
        var ret = armor ? InboundHitModifier.NO_STUN : InboundHitModifier.NONE;
        armor = false;
        return ret;
    }

    [AnimationEventHandler("BeginArmor")]
    public virtual void BeginArmor(AnimationEventData data) {
        armor = true;
        stateData.renderColorData.lerpSpeed = 20f;
        stateData.renderColorData.white = Color.red;
        stateData.renderColorData.flags |= CharacterRenderColorData.Flag.FLICKER;
    }
    
    [AnimationEventHandler("EndArmor")]
    public virtual void EndArmor(AnimationEventData data) {
        armor = false;
        stateData.renderColorData = new();
    }
}
}
