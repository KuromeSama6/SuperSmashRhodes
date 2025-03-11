using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;

namespace SuperSmashRhodes.Runtime.Tokens.State {
public abstract class State_Token_Rosmontis_Sword_Base : EntityState {
    protected Token_Rosmontis_Sword sword => (Token_Rosmontis_Sword)entity;
    public override EntityStateType type => EntityStateType.ENT_TOKEN;
    
    public State_Token_Rosmontis_Sword_Base(Entity entity) : base(entity) {
        
    }
}
}
