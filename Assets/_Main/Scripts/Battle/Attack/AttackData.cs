using SuperSmashRhodes.Battle.Serialization;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class AttackData : IReflectionSerializable {
    public EntityBBInteractionData interactionData;
    [SerializationOptions(SerializationOption.DIRECT_REFERENCE)]
    public IAttack attack;
    public Entity from;
    public Entity to;
    public AttackResult result;
    
    public ReflectionSerializer reflectionSerializer { get; }
    public AttackData() {
        reflectionSerializer = new(this);
    }
    
}


public struct AttackFrameData {
    public int startup, active, recovery, onHit, onBlock;
    public int total => startup + active + recovery;
}
}
