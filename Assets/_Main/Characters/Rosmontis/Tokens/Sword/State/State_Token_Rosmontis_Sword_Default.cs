using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;

namespace SuperSmashRhodes.Runtime.Tokens.State {
[NamedToken("Token_Rosmontis_Sword_Default")]
public class State_Token_Rosmontis_Sword_Default : State_Token_Rosmontis_Sword_Base {
    public State_Token_Rosmontis_Sword_Default(Entity entity) : base(entity) {
        
    }
    
    public override EntityStateType type => EntityStateType.ENT_TOKEN;
    
    public override EntityStateSubroutine BeginMainSubroutine() {
        return c => c.Repeat();
    }
}
}
