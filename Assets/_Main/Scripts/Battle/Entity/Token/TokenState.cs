using System.Collections;
using SuperSmashRhodes.Battle.State;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public abstract class TokenState : EntityState {
    public override EntityStateType type => EntityStateType.ENT_TOKEN;
    public TokenState(Entity entity) : base(entity) {
        
    }

    protected override void OnStateEnd() {
        base.OnStateEnd();
        entity.owner.DestroySummon(entity);
    }
}
}
