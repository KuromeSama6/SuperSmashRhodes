using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Chen_NmlAtk5D")]
public class State_Chen_NmlAtk5D : State_Chen_DriveAttack {
    public State_Chen_NmlAtk5D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_5D;
    public override float inputPriority => 3f;
    protected override string mainAnimation => "cmn/NmlAtk5D";
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.D, InputFrameType.PRESSED)
    };
    public override AttackFrameData frameData => new() {
        startup = 12, active = 14, recovery = 26,
        onHit = +10, onBlock = driveRelease ? +5 : -6
    };

    public override AttackType invincibility {
        get {
            var ret = AttackType.PROJECTILE;
            if ((driveRelease || feint) && phase == AttackPhase.ACTIVE) ret |= AttackType.FULL;
            return ret;
        }
    }

    public override StateIndicatorFlag stateIndicator => driveRelease && phase == AttackPhase.ACTIVE ? StateIndicatorFlag.INVINCIBLE : StateIndicatorFlag.NONE;

    private bool canBrake;
    private bool feint;
    private Vector3 startPos;
    
    protected override void OnStateBegin() {
        base.OnStateBegin();
        canBrake = false;
        feint = false;
    }

    protected override void OnStartup() {
        base.OnStartup();

        feint = GetCurrentInputBuffer().thisFrame.HasInput(player.side, InputType.BACKWARD, InputFrameType.HELD);
        startPos = player.transform.position;
        stateData.shouldApplySlotNeutralPose = true;
        player.audioManager.PlaySound("chr/chen/battle/sfx/drive/p1");
    }

    protected override void OnActive() {
        base.OnActive();
        stateData.physicsPushboxDisabled = true;
        player.ApplyForwardVelocity(new Vector2(driveRelease ? 40 : 25, 0));

        entity.audioManager.PlaySound($"chr/chen/battle/vo/modal/{Random.Range(0, 2)}");
        if (!feint) {
            entity.audioManager.PlaySound("chr/chen/battle/sfx/skl_214h/0", .4f);   
        }
        
        if (feint) {
            hitsRemaining = 0;
            AddCancelOption(EntityStateType.CHR_ATK_ALL);
            player.ApplyGroundedFrictionImmediate();
        }
        
        player.fxManager.PlayGameObjectFX("chr/chen/battle/fx/prefab/drive_dash_smoke", CharacterFXSocketType.WORLD_UNBOUND, player.transform.position, Vector3.zero, new(player.side == EntitySide.LEFT ? -1 : 1, 1, 1));
        player.fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/land/medium", CharacterFXSocketType.WORLD_UNBOUND, player.transform.position);
    }

    protected override void OnRecovery() {
        base.OnRecovery();
        player.ApplyGroundedFrictionImmediate();
        stateData.physicsPushboxDisabled = false;
        if (feint) {
            player.transform.position = startPos;
        }
    }

    public override void OnHit(Entity target) {
        base.OnHit(target);
        canBrake = true;
        TryBrake();
    }

    public override void OnContact(Entity to) {
        base.OnContact(to);
        if (driveRelease) {
            canBrake = true;
            AddCancelOption(EntityStateType.CHR_ATK_NORMAL | EntityStateType.CHR_ATK_SPECIAL_SUPER);
        }
        BackgroundUIManager.inst.Flash(0.03f);
        opponent.fxManager.PlayGameObjectFX("chr/chen/battle/fx/prefab/skl_214s/hit/0", CharacterFXSocketType.SELF);
        
        SimpleCameraShakePlayer.inst.Play("chr/chen/battle/fx/camerashake/drive", "5d");
    }

    protected override void OnTick() {
        base.OnTick();
        TryBrake();
    }
    
    public override float GetUnscaledDamage(Entity to) {
        return 35;
    }
    
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return blocked ? new(3f, 0f) : new(0f, driveRelease ? 12f : 9f);
    }

    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }

    private void TryBrake() {
        if (canBrake && (GetCurrentInputBuffer().thisFrame.HasInput(player.side, InputType.BACKWARD, InputFrameType.HELD) || driveRelease)) {
            player.ApplyGroundedFrictionImmediate();
            stateData.physicsPushboxDisabled = false;
            canBrake = false;
        }
    }
}
}
