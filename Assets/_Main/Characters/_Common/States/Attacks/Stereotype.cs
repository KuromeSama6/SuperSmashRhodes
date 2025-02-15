using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_NormalAttack : CharacterAttackStateBase {
    protected State_Common_NormalAttack(Entity entity) : base(entity) { }
    protected override int normalInputBufferLength => 6;
    protected override float inputMeter => 0;
    
    public override float GetChipDamagePercentage(Entity to) {
        return 0;
    }
    public override float GetOtgDamagePercentage(Entity to) {
        return .8f;
    }
}

public abstract class State_Common_AirNormalAttack : State_Common_NormalAttack {
    protected State_Common_AirNormalAttack(Entity entity) : base(entity) { }
    protected override AttackAirOkType airOk => AttackAirOkType.AIR;
    
    public override void OnContact(Entity to) {
        base.OnContact(to);
        AddCancelOption("CmnJump");
    }
    
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.STANDING;
    }
}

public abstract class State_Common_SpecialAttack : CharacterAttackStateBase {
    protected State_Common_SpecialAttack(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_SPECIAL;
    public override float inputPriority => 5f;
    protected override EntityStateType commonCancelOptions => 0;
    protected override int normalInputBufferLength => 10;
    protected override float inputMeter => 1f;

    public override float GetChipDamagePercentage(Entity to) {
        return .25f;
    }
    public override float GetOtgDamagePercentage(Entity to) {
        return .8f;
    }
    public override float GetComboProration(Entity to) {
        return .8f;
    }
    public override float GetFirstHitProration(Entity to) {
        return .8f;
    }
}

public abstract class State_Common_SummonOnlySpecialAttack : CharacterAttackStateBase {
    protected State_Common_SummonOnlySpecialAttack(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_SPECIAL;
    public override float inputPriority => 5f;
    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_SUPER;
    protected override int normalInputBufferLength => 10;
    
    public override float GetUnscaledDamage(Entity to) {
        return 0;
    }
    public override float GetChipDamagePercentage(Entity to) {
        return 0;
    }
    public override float GetOtgDamagePercentage(Entity to) {
        return 0;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return Vector2.zero;
    }
    public override float GetComboProration(Entity to) {
        return 0;
    }
    public override float GetFirstHitProration(Entity to) {
        return 0;
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override int GetFreezeFrames(Entity to) {
        return 0;
    }
    public override int GetAttackLevel(Entity to) {
        return 0;
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.EXSMALL;
    }
}

public abstract class State_Common_UtilityMove : CharacterAttackStateBase {
    public State_Common_UtilityMove(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_SPECIAL;
    protected override int normalInputBufferLength => 10;
    protected override EntityStateType commonCancelOptions => EntityStateType.CHR_ATK_SUPER;
    public override float GetUnscaledDamage(Entity to) {
        return 0;
    }
    public override float GetChipDamagePercentage(Entity to) {
        return 0;
    }
    public override float GetOtgDamagePercentage(Entity to) {
        return 0;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return Vector2.zero;
    }
    public override float GetComboProration(Entity to) {
        return 0;
    }
    public override float GetFirstHitProration(Entity to) {
        return 0;
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.ALL;
    }
    public override int GetFreezeFrames(Entity to) {
        return 0;
    }
    public override int GetAttackLevel(Entity to) {
        return 0;
    }
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.EXSMALL;
    }
}

public abstract class State_Common_CommandThrow : ThrowAttackStateBase {
    public State_Common_CommandThrow(Entity entity) : base(entity) { }
    protected override float inputMeter => 1;
    public override float GetUnscaledDamage(Entity to) {
        return 0;
    }
    protected override bool ClashableWith(ThrowAttackStateBase other) {
        return false;
    }

    protected override bool MayHit(PlayerCharacter other) {
        return base.MayHit(other) && !other.airborne;
    }
}
}
