using System.Collections;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Exusiai_DriveAttack : State_Exusiai_FireWeaponAttack {
    protected int bulletsShot;
    protected int loopFrame;
    protected int loopLength => loopEndFrame - loopStartFrame + 1;

    private int audioLoopHandle;
    
    public override AttackFrameData frameData => new AttackFrameData() {
        startup = 16,
        active = 2,
        recovery = 29
    };
    
    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_DRIVE_SPECIAL_SUPER;
    public override float GetUnscaledDamage(Entity to) {
        var chargePercentage = Mathf.Clamp01((bulletsShot + 1) / 7f);
        return 23 * chargePercentage;
    }

    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    
    public State_Exusiai_DriveAttack(Entity entity) : base(entity) {
        
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        bulletsShot = 0;
        loopFrame = 0;
    }

    public override EntityStateSubroutine BeginMainSubroutine() {
        return Sub_Startup;
    }

    protected override void Sub_Active(SubroutineContext ctx) {
        phase = AttackPhase.ACTIVE;
        OnActive();
        player.ApplyGroundedFriction(frameData.active);
        
        bulletsShot = 1;

        if (gauge.mayFire) {
            gauge.Fire();
            audioLoopHandle = entity.audioManager.PlaySoundLoop("chr/exusiai/battle/sfx/gun_loop", 0.5f, true);
            ctx.Next(0, Sub_FireLoop);
            
        } else {
            hitsRemaining = 0;
            entity.audioManager.PlaySound("chr/exusiai/battle/sfx/gun_empty");
            ctx.Next(frameData.active, Sub_RecoveryStart);
        }
    }

    protected override void Sub_RecoveryStart(SubroutineContext ctx) {
        base.Sub_RecoveryStart(ctx);
        entity.audioManager.StopSound(audioLoopHandle, "chr/exusiai/battle/sfx/gun_loop_tail", 0.5f);
    }

    protected virtual void Sub_FireLoop(SubroutineContext ctx) {
        ++loopFrame;
        if (loopFrame >= loopLength) {
            if (gauge.mayFire) {
                gauge.Fire();
                OnShotFired();
                ++hitsRemaining;
                ++bulletsShot;
                loopFrame = 0;
                entity.animation.SetFrame(loopStartFrame);
                        
            } else {
                hitsRemaining = 0;
                entity.audioManager.PlaySound("chr/exusiai/battle/sfx/gun_empty");
                ctx.Next(0, Sub_RecoveryStart);
            }
        }

        if (GetCurrentInputBuffer().thisFrame.HasInput(entity.side, InputType.D, InputFrameType.HELD)) {
            ctx.Repeat();
        } else {
            ctx.Next(0, Sub_RecoveryStart);
        }
    }

    public override float GetAtWallPushbackMultiplier(Entity to) {
        return 0;
    }

    public override float GetComboDecayIncreaseMultiplier(Entity to) {
        return 0.2f;
    }

    public override float GetMinimumDamagePercentage(Entity to) {
        return 0.05f;
    }
    public override float GetChipDamagePercentage(Entity to) {
        return 0.01f;
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.EXSMALL;
    }
    
    public override void OnContact(Entity target) {
        base.OnContact(target);
        AddCancelOption("CmnJump");
    }
    
    protected override int normalInputBufferLength => 6;
    protected virtual int loopStartFrame => 16;
    protected virtual int loopEndFrame => 19;
    protected virtual void OnShotFired() {}
}
}
