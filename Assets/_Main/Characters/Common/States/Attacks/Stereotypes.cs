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
    
    public override float GetChipDamagePercentage(Entity to) {
        return 0;
    }
    public override float GetOtgDamagePercentage(Entity to) {
        return .4f;
    }
}
}
