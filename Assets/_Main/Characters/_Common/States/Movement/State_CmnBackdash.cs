using System.Collections;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnBackdash")]
public class State_CmnBackdash : CharacterState {
    public State_CmnBackdash(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_MOVEMENT_SINGLE;
    public override float inputPriority => 1.5f;
    public override bool IsInputValid(InputBuffer buffer) {
        if (buffer.thisFrame.HasInput(entity.side, InputType.BACKWARD, InputFrameType.HELD) && buffer.TimeSlice(3).ScanForInput(entity.side, new InputFrame(InputType.DASH, InputFrameType.PRESSED))) return true;
        var ret = buffer.TimeSlice(15).ScanForInput(entity.side, InputType.BACKWARD, InputFrameType.PRESSED, 2);
        return ret;
    }
    public override AttackType invincibility => invincible ? AttackType.STRIKE | AttackType.THROW : AttackType.NONE;
    private bool invincible;
    
    protected override void OnStateBegin() {
        base.OnStateBegin();
        invincible = true;
        player.airborne = true;
        player.rb.linearVelocity = Vector2.zero;

        var velocity = player.characterConfig.backdashVelocityFinal;
        if (player.atWall) {
            player.rb.AddForceY(velocity.y, ForceMode2D.Impulse);
        } else {
            player.rb.AddForce(player.TranslateDirectionalForce(velocity), ForceMode2D.Impulse);
        }
        
        player.animation.AddUnmanagedAnimation("std/backdash", false);
        
        // burst penalty
        if (player.backdashCooldown > 75) {
            player.burst.AddDeltaTotal(-20, 120);
        }
        player.backdashCooldown += 90;

        // player.neutralAniTransitionOverride = 0f;
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/dash_dust", CharacterFXSocketType.WORLD_UNBOUND, player.transform.position, Vector3.zero, new Vector3(player.side == EntitySide.LEFT ? 1 : -1, 1, 1));
    }
    
    public override EntityStateSubroutine BeginMainSubroutine() {
        return Sub_Loop;
    }

    protected virtual void Sub_Loop(SubroutineContext ctx) {
        if (!player.airborne) {
            ctx.Exit();
            return;
        }
        
        if (frame > player.characterConfig.backdashInvulnFinal) invincible = false;
        ctx.Repeat();
    }
    
}
}
