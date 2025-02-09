using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.Tokens {
public class Token_Exusiai_ApplePie : Token {
    public override TokenFlag flags => TokenFlag.DESTROY_ON_OWNER_DAMAGE;
    protected override EntityState GetDefaultState() {
        return new State_Token_Exusiai_ApplePie_Main(this);
    }
}

public class State_Token_Exusiai_ApplePie_Main : TokenState {
    public State_Token_Exusiai_ApplePie_Main(Entity entity) : base(entity) { }
    public override IEnumerator MainRoutine() {
        yield break;
    }
}
}