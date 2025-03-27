using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Animation;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Chen_DriveAttack : CharacterAttackStateBase {
    public State_Chen_DriveAttack(Entity entity) : base(entity) { }
    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_SPECIAL_SUPER;
    protected override int normalInputBufferLength => 6;
    protected override float inputMeter => 0;

    protected override void OnStartup() {
        base.OnStartup();
        stateData.shouldApplySlotNeutralPose = true;
        entity.PlaySound("chr/chen/battle/sfx/drive/p1");
    }

    protected override void OnActive() {
        base.OnActive();
        stateData.ghostFXData = new("cb0000".HexToColor(), 0.02333334f, driveRelease ? 40 : 20, 2.5f);
        stateData.physicsPushboxDisabled = true;

        entity.PlaySound($"chr/chen/battle/vo/modal/{random.Range(0, 2)}");
        
        player.fxManager.PlayGameObjectFX("chr/chen/battle/fx/prefab/drive_dash_smoke", CharacterFXSocketType.WORLD_UNBOUND, player.transform.position, Vector3.zero, new(player.side == EntitySide.LEFT ? -1 : 1, 1, 1));
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/land/medium", CharacterFXSocketType.WORLD_UNBOUND, player.transform.position);
    }

    protected override void OnRecovery() {
        base.OnRecovery();
        stateData.ghostFXData = null;
        player.ApplyGroundedFrictionImmediate();
        stateData.physicsPushboxDisabled = false;
        
    }

    public override void OnContact(Entity to) {
        base.OnContact(to);
        BackgroundUIManager.inst.Flash(0.03f);
        opponent.fxManager.PlayGameObjectFX("chr/chen/battle/fx/prefab/skl_214s/hit/0", CharacterFXSocketType.SELF);
        SimpleCameraShakePlayer.inst.Play("chr/chen/battle/fx/camerashake", "5d");
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
    public override int GetAttackLevel(Entity to) {
        return 3;
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.MEDIUM;
    }

    public override Vector2 GetCarriedMomentumPercentage(Entity to) {
        // return new(1f, 0.2f);
        return Vector2.zero;
    }

    [AnimationEventHandler("BeginTeleportSlashSubroutine")]
    public virtual void OnBeginTeleportSlashSubroutine() {
        
    }
}

}
