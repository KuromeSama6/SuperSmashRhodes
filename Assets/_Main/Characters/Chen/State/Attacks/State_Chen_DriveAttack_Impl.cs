using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Animation;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Chen_NmlAtk5D")]
public class State_Chen_NmlAtk5D : State_Chen_DriveAttack {
    public State_Chen_NmlAtk5D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_5D;
    public override float inputPriority => 3f;
    protected override string mainAnimation => "cmn/NmlAtk5D";
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.D, InputFrameType.PRESSED)
    };
    public override AttackFrameData frameData => new() {
        startup = 12, active = 14, recovery = 26,
        onHit = +10, onBlock = driveRelease ? +5 : -6
    };

    public override AttackType invincibility {
        get {
            var ret = AttackType.PROJECTILE;
            if ((driveRelease || feint) && phase == AttackPhase.ACTIVE) ret |= AttackType.FULL;
            return ret;
        }
    }

    public override StateIndicatorFlag stateIndicator => driveRelease && phase == AttackPhase.ACTIVE ? StateIndicatorFlag.INVINCIBLE : StateIndicatorFlag.NONE;

    protected bool canBrake;
    private bool feint;
    private Vector3 startPos;
    private bool blocked;
    
    protected override void OnStateBegin() {
        base.OnStateBegin();
        canBrake = false;
        feint = false;
        blocked = false;
    }

    protected override void OnStartup() {
        base.OnStartup();

        feint = !player.airborne && GetCurrentInputBuffer().thisFrame.HasInput(player.side, InputType.BACKWARD, InputFrameType.HELD);
        startPos = player.transform.position;
    }

    protected override void OnActive() {
        base.OnActive();
        
        player.ApplyForwardVelocity(new Vector2(driveRelease ? 40 : 20, 0));
        
        if (feint) {
            hitsRemaining = 0;
            AddCancelOption(EntityStateType.CHR_ATK_ALL);
            player.ApplyGroundedFrictionImmediate();
            
        } else {
            entity.audioManager.PlaySound("chr/chen/battle/sfx/skl_214h/0", .4f);   
        }
    }

    protected override void OnRecovery() {
        base.OnRecovery();
        if (feint) {
            player.transform.position = startPos;
        }
        
    }

    public override void OnHit(Entity target) {
        base.OnHit(target);
        canBrake = true;
        TryBrake();
    }


    public override void OnBlock(Entity target) {
        base.OnBlock(target);
        canBrake = true;
        blocked = true;
        TryBrake();
    }

    public override void OnContact(Entity to) {
        base.OnContact(to);
        if (driveRelease) {
            canBrake = true;
            AddCancelOption(EntityStateType.CHR_ATK_NORMAL | EntityStateType.CHR_ATK_SPECIAL_SUPER);
        }
    }

    protected override void OnTick() {
        base.OnTick();
        TryBrake();
    }
    
    public override float GetUnscaledDamage(Entity to) {
        return 35;
    }
    
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return blocked ? new(3f, 0f) : new(0f, driveRelease ? 12f : 9f);
    }

    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }

    protected void TryBrake() {
        if (canBrake && (GetCurrentInputBuffer().thisFrame.HasInput(player.side, InputType.BACKWARD, InputFrameType.HELD) || driveRelease || blocked)) {
            player.ApplyGroundedFrictionImmediate();
            stateData.physicsPushboxDisabled = false;
            canBrake = false;
        }
    }
}

[NamedToken("Chen_NmlAtk6D")]
public class State_Chen_NmlAtk6D : State_Chen_DriveAttack {
    public State_Chen_NmlAtk6D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_6D;
    public override float inputPriority => 3.1f;
    protected override string mainAnimation => "cmn/NmlAtk6D";
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.FORWARD, InputFrameType.HELD),
        new InputFrame(InputType.D, InputFrameType.PRESSED)
    };
    public override AttackFrameData frameData => new() {
        startup = 16, active = 14, recovery = 26,
        onHit = +10, onBlock = driveRelease ? +5 : -6
    };

    public override AttackType invincibility {
        get {
            var ret = AttackType.PROJECTILE;
            if ((driveRelease) && phase == AttackPhase.ACTIVE) ret |= AttackType.FULL;
            return ret;
        }
    }
    public override LandingRecoveryFlag landingRecoveryFlag => driveRelease ? LandingRecoveryFlag.UNTIL_LAND : LandingRecoveryFlag.NONE;

    public override StateIndicatorFlag stateIndicator => driveRelease && phase == AttackPhase.ACTIVE ? StateIndicatorFlag.INVINCIBLE : StateIndicatorFlag.NONE;

    
    protected override void OnStateBegin() {
        base.OnStateBegin();
    }

    protected override void OnStartup() {
        base.OnStartup();
    }

    protected override void OnActive() {
        base.OnActive();
        
        player.ApplyForwardVelocity(new Vector2(driveRelease ? 30 : 15, 11));
        entity.audioManager.PlaySound("chr/chen/battle/sfx/skl_214h/0", .4f);
        player.airborne = true;
    }

    protected override void OnRecovery() {
        base.OnRecovery();
    }

    public override void OnHit(Entity target) {
        base.OnHit(target);
        stateData.gravityScale = 3f;
    }


    public override void OnBlock(Entity target) {
        base.OnBlock(target);
    }

    public override void OnContact(Entity to) {
        base.OnContact(to);
        player.ApplyGroundedFrictionImmediate();
        stateData.physicsPushboxDisabled = false;
    }

    protected override void OnTick() {
        base.OnTick();
    }
    
    public override float GetUnscaledDamage(Entity to) {
        return 41;
    }
    
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return blocked ? new(2f, 0f) : new(0.1f, driveRelease ? 12 : 10f);
    }

    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
}

[NamedToken("Chen_NmlAtk8D")]
public class State_Chen_NmlAtk8D : State_Chen_NmlAtk5D {
    public State_Chen_NmlAtk8D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_8D;
    public override float inputPriority => 3f;
    protected override string mainAnimation => "cmn/NmlAtk5D";
    protected override AttackAirOkType airOk => AttackAirOkType.AIR;
    public override AttackFrameData frameData => new() {
        startup = 16, active = 14, recovery = 26,
        onHit = +10, onBlock = driveRelease ? +1 : -10
    };

    public override LandingRecoveryFlag landingRecoveryFlag => LandingRecoveryFlag.UNTIL_LAND;

    protected override void OnStartup() {
        base.OnStartup();
    }

    protected override void OnActive() {
        player.rb.linearVelocity = Vector2.zero;
        base.OnActive();
        stateData.gravityScale = 0;
    }

    public override void OnContact(Entity to) {
        base.OnContact(to);
        canBrake = true;
        TryBrake();
        // stateData.gravityScale = 1;
    }

    protected override void OnRecovery() {
        base.OnRecovery();
        stateData.gravityScale = 1;
    }
    

    public override void OnHit(Entity target) {
        base.OnHit(target);
        AddCancelOption("CmnJump");
    }

    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.STANDING;
    }

}

[NamedToken("Chen_NmlAtk82D")]
public class Chen_NmlAtk82D : State_Chen_DriveAttack {
    public Chen_NmlAtk82D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_82D;
    public override float inputPriority => 3.1f;
    protected override string mainAnimation => "cmn/NmlAtk82D";
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.DOWN, InputFrameType.HELD),
        new InputFrame(InputType.D, InputFrameType.PRESSED)
    };
    public override AttackFrameData frameData => new() {
        startup = 12, active = 14, recovery = 26,
        onHit = +10, onBlock = -40
    };

    public override AttackType invincibility {
        get {
            var ret = AttackType.PROJECTILE;
            if ((driveRelease) && phase == AttackPhase.ACTIVE) ret |= AttackType.FULL;
            return ret;
        }
    }
    public override LandingRecoveryFlag landingRecoveryFlag => driveRelease ? LandingRecoveryFlag.NONE : LandingRecoveryFlag.UNTIL_LAND;
    protected override AttackAirOkType airOk => AttackAirOkType.AIR;
    public override StateIndicatorFlag stateIndicator => driveRelease && phase == AttackPhase.ACTIVE ? StateIndicatorFlag.INVINCIBLE : StateIndicatorFlag.NONE;

    
    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.frameData.landingFlag = LandingRecoveryFlag.UNTIL_LAND;
        player.frameData.landingRecoveryFrames = frameData.recovery;
    }

    protected override void OnStartup() {
        base.OnStartup();
    }

    protected override void OnActive() {
        base.OnActive();
        
        player.ApplyForwardVelocity(new Vector2(driveRelease ? 30 : 15, -11));
        entity.audioManager.PlaySound("chr/chen/battle/sfx/skl_214h/0", .4f);
        player.airborne = true;
    }

    protected override void OnRecovery() {
        base.OnRecovery();
    }

    public override void OnHit(Entity target) {
        base.OnHit(target);
        stateData.gravityScale = 3f;
        player.opponent.frameData.AddGroundBounce(new Vector2(0, driveRelease ? 10f : 5f));
        player.opponent.ForceSetAirborne();
    }


    public override void OnBlock(Entity target) {
        base.OnBlock(target);
    }

    public override void OnContact(Entity to) {
        base.OnContact(to);
        player.ApplyGroundedFrictionImmediate();
        stateData.physicsPushboxDisabled = false;
    }

    public override InboundHitModifier OnHitByOther(AttackData attackData) {
        player.rb.linearVelocity = Vector2.zero;
        return base.OnHitByOther(attackData);
    }

    protected override void OnTick() {
        base.OnTick();
    }
    
    public override float GetUnscaledDamage(Entity to) {
        return 38;
    }
    
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return blocked ? new(2f, 0f) : new(0.5f, -10f);
    }

    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.STANDING;
    }
}

}
