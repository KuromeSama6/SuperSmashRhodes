using System.Collections;
using SuperSmashRhodes.Battle;
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
        return buffer.thisFrame.HasInput(entity.side, InputType.BACKWARD, InputFrameType.HELD) &&
               buffer.TimeSlice(3).ScanForInput(entity.side, new InputFrame(InputType.DASH, InputFrameType.PRESSED));
    }
    public override bool fullyInvincible => invincible;
    private bool invincible;
    
    protected override void OnStateBegin() {
        base.OnStateBegin();
        invincible = true;
        player.airborne = true;
        player.rb.linearVelocity = Vector2.zero;

        var velocity = player.characterConfig.backdashVelocity;
        if (player.atWall) {
            player.rb.AddForceY(velocity.y, ForceMode2D.Impulse);
        } else {
            player.rb.AddForce(player.TranslateDirectionalForce(velocity), ForceMode2D.Impulse);
        }
        
        player.animation.AddUnmanagedAnimation("std/backdash", false);
        
        // burst penalty
        player.burst.AddDeltaTotal(-20, 120);
        
        // player.neutralAniTransitionOverride = 0f;
    }
    
    public override IEnumerator MainRoutine() {
        while (player.airborne) {
            if (frame > player.characterConfig.backdashInvuln) invincible = false;
            yield return 1;
        }
    }
}
}
