using System.Collections;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Postprocess;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Scripts.Audio;
using SuperSmashRhodes.UI.Battle;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("SysDeath")]
public class State_SysDeath : CharacterState {
    public State_SysDeath(Entity entity) : base(entity) {
        
    }
    public override EntityStateType type => EntityStateType.CHR_ATK_SYSTEMSPECIAL;

    protected override void OnStateBegin() {
        base.OnStateBegin();

        var inAir = player.lastState is State_CmnHitStunAir && player.airborne;
        
        if (!inAir) {
            player.animation.AddUnmanagedAnimation("defaults/Die", false);
            
        } else {
            player.rb.linearVelocity = Vector2.zero;
            player.ApplyForwardVelocity(new(-3, 7));
        }
        
        PostProcessManager.inst.showKnockout = true;
        stateData.cameraData.cameraWeightModifier = 5f;
        stateData.cameraData.cameraWeightModifier = -15f;
        
        BackgroundUIManager.inst.Flash(0.1f);
        
        TimeManager.inst.Schedule(4, 60);
        TimeManager.inst.Queue(() => {
            player.slowdownFrames = 60;
            AudioManager.inst.PlayAudioClip("cmn/battle/sfx/dead", GameManager.inst.gameObject);
            
            stateData.renderColorData.white = Color.black;
            stateData.renderColorData.lerpSpeed = 5f;
        });
        
        SimpleCameraShakePlayer.inst.PlayCommon("knockout");
    }

    public override EntityStateSubroutine BeginMainSubroutine() {
        return Sub_Loop;
    }

    protected virtual void Sub_Loop(SubroutineContext ctx) {
        ctx.Repeat();
    }

    protected override void OnTick() {
        base.OnTick();
        if (frame == 30) {
            PostProcessManager.inst.showKnockout = false;
        }

        if (frame == 45) {
            stateData.renderColorData.white = Color.clear;
        }
        
        if (frame == 90) {
            stateData.cameraData.cameraWeightModifier = 0f;
            player.stateFlags |= CharacterStateFlag.NO_CAMERA_WEIGHT;
            stateData.cameraData.cameraFovModifier = 0f;
            
        }
    }

    public override float inputPriority => 0;
    public override bool IsInputValid(InputBuffer buffer) {
        return false;
    }
}
}
