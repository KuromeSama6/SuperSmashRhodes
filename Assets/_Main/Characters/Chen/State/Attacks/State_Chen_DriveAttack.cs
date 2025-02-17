using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Animation;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Chen_DriveAttack : CharacterAttackStateBase {
    public State_Chen_DriveAttack(Entity entity) : base(entity) { }
    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_SPECIAL_SUPER;
    protected override int normalInputBufferLength => 6;
    protected override float inputMeter => 0;

    protected override void OnActive() {
        base.OnActive();
        stateData.ghostFXData = new("cb0000".HexToColor(), 0.02333334f, driveRelease ? 40 : 20, 2.5f);
    }

    protected override void OnRecovery() {
        base.OnRecovery();
        stateData.ghostFXData = null;
    }

    public override float GetChipDamagePercentage(Entity to) {
        return .05f;
    }
    public override float GetOtgDamagePercentage(Entity to) {
        return .6f;
    }
    public override float GetComboProration(Entity to) {
        return .9f;
    }
    public override float GetFirstHitProration(Entity to) {
        return 1f;
    }
    public override int GetFreezeFrames(Entity to) {
        return 0;
    }
    public override int GetAttackLevel(Entity to) {
        return 3;
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.MEDIUM;
    }

    public override Vector2 GetCarriedMomentumPercentage(Entity to) {
        return new(1f, 0.2f);
    }

    [AnimationEventHandler("BeginTeleportSlashSubroutine")]
    public virtual void OnBeginTeleportSlashSubroutine() {
        
    }
}

}
