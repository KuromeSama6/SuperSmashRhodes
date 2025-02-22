using SuperSmashRhodes.Battle.Serialization;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class AttackData : IHandleSerializable {
    public EntityBBInteractionData interactionData;
    public IAttack attack;
    public Entity from;
    public Entity to;
    public AttackResult result;

    public IHandle GetHandle() {
        return new Handle(this);
    }

    private struct Handle : IHandle {
        private EntityBBInteractionData interactionData;
        private IAttack attack;
        private EntityHandle from;
        private EntityHandle to;
        private AttackResult result;
        
        public Handle(AttackData data) {
            interactionData = data.interactionData;
            attack = data.attack;
            from = (EntityHandle)data.from.GetHandle();
            to = (EntityHandle)data.to.GetHandle();
            result = data.result;
        }
        
        public object Resolve() {
            var data = new AttackData();
            data.interactionData = interactionData;
            data.attack = attack;
            data.from = (Entity)from.Resolve();
            data.to = (Entity)to.Resolve();
            data.result = result;
            return data;
        }
    }
    
}

public struct AttackFrameData {
    public int startup, active, recovery, onHit, onBlock;
    public int total => startup + active + recovery;
}
}
