using MoreMountains.Feedbacks;
using Spine.Unity.Examples;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Animation;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Runtime.Tokens;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Exusiai_SklApplePie")]
public class State_Exusiai_SklApplePie : State_Common_SummonOnlySpecialAttack {
    private Token_Exusiai_ApplePie applePie;
    
    public State_Exusiai_SklApplePie(Entity entity) : base(entity) {
        
    }
    protected override string mainAnimation => "chr/SklApplePie";
    protected override float inputMeter => 1f;
    public override AttackFrameData frameData => new AttackFrameData() {
        startup = 11, active = 12, recovery = 14
    };

    public override Hitstate hitstate => Hitstate.COUNTER;

    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.DOWN, InputFrameType.HELD), new InputFrame(InputType.FORWARD, InputFrameType.PRESSED), new InputFrame(InputType.HS, InputFrameType.PRESSED)
    };

    protected override void OnStateBegin() {
        base.OnStateBegin();
        applePie = null;
        entity.audioManager.PlaySound("chr/exusiai/battle/vo/modal/3");
    }

    protected override void OnStateEnd(EntityState nextState) {
        base.OnStateEnd(nextState);
        if (applePie.attached) entity.CallLaterCoroutine(0.1f, () => {
            applePie.Detach();
            applePie.rb.AddForce(PhysicsUtil.NormalizeSide(new Vector2(2f, 2f), entity.side), ForceMode2D.Impulse);
        });
    }

    [AnimationEventHandler("ApplePie_Summon")]
    public virtual void OnApplePieSummon() {
        applePie = entity.Summon<Token_Exusiai_ApplePie>("chr/exusiai/battle/token/ApplePie");
        entity.SetCarriedStateVariable("Token_ApplePie", null, applePie);
    }

    [AnimationEventHandler("ApplePie_Detach")] 
    public virtual void OnApplePieDetach() {
        applePie.Detach();
        AddCancelOption("Exusiai_SklApplePie_FDetonate");
        applePie.rb.AddForce(PhysicsUtil.NormalizeSide(new Vector2(2f, 2f), entity.side), ForceMode2D.Impulse);
    }
}

[NamedToken("Exusiai_SklApplePie_FDetonate")]
public class State_Exusiai_SklApplePie_FDetonate : State_Common_SummonOnlySpecialAttack {
    public State_Exusiai_SklApplePie_FDetonate(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_SPECIAL_TRIGGER;
    protected override string mainAnimation => "chr/SklApplePie_FDetonate";
    protected override float inputMeter => 1.5f;
    public override AttackFrameData frameData => new AttackFrameData() {
        startup = 9, active = 15, recovery = 12
    };
    public override float inputPriority => 5;
    protected override InputFrame[] requiredInput => new[] { new InputFrame(InputType.BACKWARD, InputFrameType.HELD), new InputFrame(InputType.HS, InputFrameType.PRESSED)};
    
    [AnimationEventHandler("ApplePie_Detonate")]
    private void OnApplePieDetonate() {
        // Debug.Log("detonate");
        if (stateData.TryGetCarriedVariable("Token_ApplePie", out Token_Exusiai_ApplePie pie)) {
            ((State_Token_Exusiai_ApplePie_Main)pie.activeState).DetonateImmediate();
        }
    }
}
}
