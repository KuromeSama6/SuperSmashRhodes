using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
public abstract class State_Common_NormalAttack : CharacterAttackStateBase {
    protected State_Common_NormalAttack(Entity owner) : base(owner) { }
    protected override int normalInputBufferLength => 6;
    
    public override float GetChipDamagePercentage(Entity to) {
        return 0;
    }
    public override float GetOtgDamagePercentage(Entity to) {
        return .8f;
    }
}

public abstract class State_Common_SpecialAttack : CharacterAttackStateBase {
    protected State_Common_SpecialAttack(Entity owner) : base(owner) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_SPECIAL;
    public override float inputPriority => 5f;
    protected override EntityStateType commonCancelOptions => 0;
    protected override int normalInputBufferLength => 10;

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
}
