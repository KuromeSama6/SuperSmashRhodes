﻿using System.Collections;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {

[NamedToken("CmnBlockStun")]
public class State_CmnBlockStun : State_Common_Stun {
    public State_CmnBlockStun(Entity entity) : base(entity) { }
    protected override int frames => player.frameData.blockstunFrames;
    protected override string animationName => "std/blockstun";
    public override EntityStateType type => EntityStateType.CHR_BLOCKSTUN;
    public override bool mayEnterState => entity.activeState is State_CmnBlockStunCrouch;
    public override bool IsInputValid(InputBuffer buffer) {
        return !buffer.thisFrame.HasInput(entity.side, InputType.DOWN, InputFrameType.HELD);
    }
    protected override void OnStateBegin() {
        base.OnStateBegin();
        AddCancelOption("CmnBlockStunCrouch");
        AddCancelOption("CmnDriveRelease");
        // player.ApplyGroundedFrictionImmediate();
    }
}

[NamedToken("CmnBlockStunCrouch")]
public class State_CmnBlockStunCrouch : State_Common_Stun {
    public State_CmnBlockStunCrouch(Entity entity) : base(entity) { }
    protected override int frames => player.frameData.blockstunFrames;
    protected override string animationName => "std/blockstun_crouch";
    public override EntityStateType type => EntityStateType.CHR_BLOCKSTUN;

    public override bool mayEnterState => entity.activeState is State_CmnBlockStun;
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.thisFrame.HasInput(entity.side, InputType.DOWN, InputFrameType.HELD);
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        AddCancelOption("CmnBlockStun");
        AddCancelOption("CmnDriveRelease");
        // player.ApplyGroundedFrictionImmediate();
    }

    public override EntityStateSubroutine BeginMainSubroutine() {
        AddCancelOption("CmnDriveRelease");
        return Sub_StunLoop;
    }

    protected override void Sub_StunLoop(SubroutineContext ctx) {
        if (frames > 0) {
            player.ApplyGroundedFriction();
            if (!RevalidateInput()) {
                CancelInto("CmnBlockStun");
                return;
            }
            ctx.Repeat();
            return;
        }
        
        ctx.Exit();
    }

}

[NamedToken("CmnBlockStunAir")]
public class State_CmnBlockStunAir : State_Common_Stun {
    public State_CmnBlockStunAir(Entity entity) : base(entity) { }
    protected override int frames => player.frameData.blockstunFrames;
    protected override string animationName => "std/blockstun";
    public override EntityStateType type => EntityStateType.CHR_BLOCKSTUN;
    public override bool mayEnterState => entity.activeState is State_CmnBlockStunCrouch;
    public override bool IsInputValid(InputBuffer buffer) {
        return false;
    }
    protected override void OnStateBegin() {
        base.OnStateBegin();
        AddCancelOption("CmnBlockStun");
        AddCancelOption("CmnDriveRelease");
    }

    public override EntityStateSubroutine BeginMainSubroutine() {
        return ctx => ctx.Repeat();
    }

    public override void OnLand(LandingRecoveryFlag flag, int recoveryFrames) {
        base.OnLand(flag, recoveryFrames);
        //19F Forced blockstun
        player.frameData.blockstunFrames = 19;
        CancelInto("CmnBlockStun");
    }
}
}
