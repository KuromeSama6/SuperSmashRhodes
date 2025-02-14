using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_NmlAtkAirThrow : ThrowAttackStateBase {
    public State_Common_NmlAtkAirThrow(Entity entity) : base(entity) { }
    public override float inputPriority => 5f;
    public override EntityStateType type => EntityStateType.CHR_ATK_THROW;
    public override bool IsInputValid(InputBuffer buffer) {
        // return buffer.TimeSlice(3).ScanForInput(owner.side, new InputFrame(InputType.D, InputFrameType.PRESSED), new InputFrame(InputType.P, InputFrameType.PRESSED)); 
        return buffer.TimeSlice(3).HasInputUnordered(entity.side, new InputFrame(InputType.D, InputFrameType.PRESSED), new InputFrame(InputType.P, InputFrameType.PRESSED)); 
    }
    protected override string mainAnimation => "cmn/NmlAtkGndThrow";
    protected override string whiffAnimation => "cmn/NmlAtkGndThrow_W";
    protected override float inputMeter => 0;
    protected override AttackAirOkType airOk => AttackAirOkType.AIR;

    public override AttackFrameData frameData => new() {
        startup = 2,
        active = 3,
        recovery = 38
    };
    protected override float distanceRequirement => 1f;
    public override float GetUnscaledDamage(Entity to) {
        return 80f;
    }
    protected override bool ClashableWith(ThrowAttackStateBase other) {
        return other is State_Common_NmlAtkAirThrow;
    }
    protected override bool ShouldSwitchSides(PlayerCharacter other) {
        return player.inputProvider.inputBuffer.thisFrame.HasInput(entity.side, InputType.BACKWARD, InputFrameType.HELD);
    }

    protected override void OnThrowWhiff(PlayerCharacter other) {
        base.OnThrowWhiff(other);
        player.frameData.landingRecoveryFrames = 10;
        player.frameData.landingFlag |= LandingRecoveryFlag.UNTIL_LAND;
    }

    protected override void OnThrowTech(PlayerCharacter other) {
        // base.OnThrowTech(other);
        opponent.rb.linearVelocity = Vector2.zero;
        opponent.rb.AddForce(opponent.TranslateDirectionalForce(new(-3, 10)), ForceMode2D.Impulse);
        
        player.rb.linearVelocity = Vector2.zero;
        player.rb.AddForce(player.TranslateDirectionalForce(new(-3, 10)), ForceMode2D.Impulse);
        stateData.gravityScale = 1;
        
        player.ResetAirOptions();
        opponent.ResetAirOptions();
    }

    protected override void OnThrowHit(PlayerCharacter other) {
        base.OnThrowHit(other);
        player.rb.linearVelocity = Vector2.zero;
        stateData.gravityScale = 0;
    }

    protected override void OnFinalHit() {
        opponent.frameData.landingFlag |= LandingRecoveryFlag.HARD_KNOCKDOWN_LAND;
        opponent.BeginState("CmnHitStunAir");
        stateData.gravityScale = 1;
        
        // opponent.rb.AddForce(opponent.TranslateDirectionalForce(new(-10, 0)), ForceMode2D.Impulse);
        player.rb.AddForce(player.TranslateDirectionalForce(new(-5, 0)), ForceMode2D.Impulse);
    }

    protected override bool MayHit(PlayerCharacter other) {
        return base.MayHit(other) && other.airborne;
    }

}
}
