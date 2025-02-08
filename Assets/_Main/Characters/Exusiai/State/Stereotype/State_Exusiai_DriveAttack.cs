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
    
    public State_Exusiai_DriveAttack(Entity owner) : base(owner) {
        
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        bulletsShot = 0;
        loopFrame = 0;
    }

    public override IEnumerator MainRoutine() {
        owner.animation.AddUnmanagedAnimation(mainAnimation, false);
        phase = AttackPhase.STARTUP;
        OnStartup();
        
        player.ApplyGroundedFriction(frameData.startup);
        owner.audioManager.PlaySound(GetAttackNormalSfx());
        yield return frameData.startup;

        phase = AttackPhase.ACTIVE;
        OnActive();
        player.ApplyGroundedFriction(frameData.active);
        
        bulletsShot = 1;


        if (gauge.mayFire) {
            gauge.Fire();
            audioLoopHandle = owner.audioManager.PlaySoundLoop("chr/exusiai/battle/sfx/gun_loop", 0.5f);
            do {
                ++loopFrame;
                if (loopFrame >= loopLength) {
                    if (gauge.mayFire) {
                        gauge.Fire();
                        ++hitsRemaining;
                        ++bulletsShot;
                        loopFrame = 0;
                        owner.animation.SetFrame(loopStartFrame);
                        
                    } else {
                        hitsRemaining = 0;
                        owner.audioManager.PlaySound("chr/exusiai/battle/sfx/gun_empty");
                        break;
                    }
                }
                yield return 1;
            
            } while (GetCurrentInputBuffer().thisFrame.HasInput(owner.side, InputType.D, InputFrameType.HELD));
            
            owner.audioManager.PlaySound($"chr/exusiai/battle/sfx/gun_shell_{Random.Range(1, 4)}");
        } else {
            // whiff direct
            hitsRemaining = 0;
            owner.audioManager.PlaySound("chr/exusiai/battle/sfx/gun_empty");
        }
        yield return frameData.active; 
        
        owner.audioManager.StopSoundLoop(audioLoopHandle, "chr/exusiai/battle/sfx/gun_loop_tail", 0.5f);
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
    
    protected override int normalInputBufferLength => 6;
    protected abstract int loopStartFrame { get; }
    protected abstract int loopEndFrame { get; }
}
}
