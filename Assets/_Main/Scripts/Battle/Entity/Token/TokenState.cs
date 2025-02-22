using System.Collections;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Battle.State;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public abstract class TokenState : EntityState {
    private EntityHandle verificationHandle;
    
    protected TokenState(Entity entity) : base(entity) {
        verificationHandle = new(entity);
        // Debug.Log(verificationHandle);
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        EnsureEntity();
    }

    protected void EnsureEntity() {
        entity = (Entity)verificationHandle.Resolve() ?? entity;
    }

    public override void Deserialize(StateSerializer serializer) {
        base.Deserialize(serializer);
        EnsureEntity();
        // Debug.Log($"ensure ent, {entity}");
    }
}
}
