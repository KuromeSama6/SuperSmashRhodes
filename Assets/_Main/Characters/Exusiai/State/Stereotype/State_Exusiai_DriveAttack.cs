using System.Collections;
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
        recovery = 29,
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

    public override IEnumerator MainRoutine() {
        entity.animation.AddUnmanagedAnimation(mainAnimation, false);
        phase = AttackPhase.STARTUP;
        OnStartup();
        
        player.ApplyGroundedFriction(frameData.startup);
        entity.audioManager.PlaySound(GetAttackNormalSfx());
        yield return frameData.startup;

        phase = AttackPhase.ACTIVE;
        OnActive();
        player.ApplyGroundedFriction(frameData.active);
        
        bulletsShot = 1;
        
        if (gauge.mayFire) {
            gauge.Fire();
            audioLoopHandle = entity.audioManager.PlaySoundLoop("chr/exusiai/battle/sfx/gun_loop", 0.5f, true);
            do {
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
                        break;
                    }
                }
                
                // drive cancel
                var thisFrame = GetCurrentInputBuffer().thisFrame;
                // if (type != EntityStateType.CHR_ATK_8D && thisFrame.HasInput(owner.side, InputType.D, InputFrameType.HELD)) {
                //     string targetType;
                //     if (thisFrame.HasInput(owner.side, InputType.DOWN, InputFrameType.HELD)) targetType = "Exusiai_NmlAtk2D";
                //     else if (thisFrame.HasInput(owner.side, InputType.FORWARD, InputFrameType.HELD)) targetType = "Exusiai_NmlAtk6D";
                //     else if (thisFrame.HasInput(owner.side, InputType.BACKWARD, InputFrameType.HELD)) targetType = "Exusiai_NmlAtk4D";
                //     else targetType = "Exusiai_NmlAtk5D";
                //
                //     if (targetType != id) {
                //         CancelInto(targetType);
                //     }
                // }
                
                yield return 1;
            } while (GetCurrentInputBuffer().thisFrame.HasInput(entity.side, InputType.D, InputFrameType.HELD));
            
            entity.audioManager.PlaySound($"chr/exusiai/battle/sfx/gun_shell_{Random.Range(1, 4)}");
        } else {
            // whiff direct
            hitsRemaining = 0;
            entity.audioManager.PlaySound("chr/exusiai/battle/sfx/gun_empty");
        }
        yield return frameData.active; 
        
        entity.audioManager.StopSoundLoop(audioLoopHandle, "chr/exusiai/battle/sfx/gun_loop_tail", 0.5f);
        player.ApplyGroundedFriction(frameData.active);
        phase = AttackPhase.RECOVERY;
        OnRecovery();
        yield return frameData.recovery;
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
