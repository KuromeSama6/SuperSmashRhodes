using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Runtime.State;
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
    public int startup, active, recovery;
    public int total => startup + active + recovery;

    public AttackFrameData(int startup, int active, int recovery) {
        this.startup = startup;
        this.active = active;
        this.recovery = recovery;
    }
    
    public static int GetStandardStun(Entity to, bool blocked, int attackLevel) {
        if (blocked) {
            return attackLevel switch {
                0 => 9,
                1 => 11,
                2 => 13,
                3 => 16,
                4 => 18,
                _ => 18
            };

        } else {
            var ret = 11 + attackLevel;
            if (to.activeState is State_CmnNeutralCrouch) ret += 1;
            return ret;
        }
    }
}
}
