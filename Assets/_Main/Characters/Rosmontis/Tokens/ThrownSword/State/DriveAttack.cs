using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.Tokens.State {
public abstract class Token_Rosmontis_ThrownSword_DriveAttack : ProjectileAttackStateBase {
    public Token_Rosmontis_ThrownSword_DriveAttack(Entity entity) : base(entity) { }
    protected override string mainAnimation => "attack/drive";
    protected override int projectileLevel => 2;
    protected abstract float startingRotation { get; }

    protected override void OnStateBegin() {
        entity.transform.eulerAngles = new(0, 0, entity.side == EntitySide.LEFT ? startingRotation : 180 - startingRotation);
        base.OnStateBegin();
    }

    public override float GetUnscaledDamage(Entity to) {
        return 40;
    }
    
    public override float GetComboProration(Entity to) {
        return .8f;
    }
    
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }

    public override void OnContact(Entity to) {
        base.OnContact(to);
        entity.PlaySound("chr/rosmontis/battle/sfx/drive/impact");

        player.activeState.AddCancelOption(EntityStateType.CHR_ATK_DRIVE_SPECIAL_SUPER);
        player.activeState.AddCancelOption("CmnJump");
    }
}
}
