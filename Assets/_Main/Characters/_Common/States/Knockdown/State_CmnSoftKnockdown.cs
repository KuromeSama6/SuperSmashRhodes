using System.Collections;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("CmnSoftKnockdown")]
public class State_CmnSoftKnockdown : CharacterState {
    public State_CmnSoftKnockdown(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_SOFT_KNOCKDOWN;
    public override float inputPriority => -1;
    public override AttackType invincibility => AttackType.FULL;
    public override StateIndicatorFlag stateIndicator {
        get {
            var ret = StateIndicatorFlag.NONE;
            if (stateData.TryGetCarriedVariable<bool>("_fromThrowTech", out _)) {
                // ret |= StateIndicatorFlag.THROW;
                ret |= StateIndicatorFlag.THROW_TECH;
            }
            return ret;
        }
    }

    private bool isHardKnockdown;
    
    public override bool IsInputValid(InputBuffer buffer) {
        return false;
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        entity.animation.AddUnmanagedAnimation("std/ground_tech", false);
        isHardKnockdown = stateData.TryGetCarriedVariable("_hardKnockdown", out bool value) && value;

        if (isHardKnockdown) {
            // add cancel options
            AddCancelOption(EntityStateType.CHR_ATK_SPECIAL_SUPER);
        }
        
        
    }
    public override EntityStateSubroutine BeginMainSubroutine() {
        return ctx => ctx.Exit(30);
    }

    protected override void OnStateEndComplete(EntityState nextState) {
        base.OnStateEndComplete(nextState);
        player.neutralAniTransitionOverride = 0f;
        
    }

    protected override void OnStateEnd(EntityState nextState) {
        base.OnStateEnd(nextState);
        player.frameData.throwInvulnFrames = 5;
        // Debug.Log("state end");
        if (nextState is CharacterAttackStateBase) {
            // Debug.Log("rev");
            player.SetCarriedStateVariable("_isReversalMove", null, true);
        }
    }
}
}
