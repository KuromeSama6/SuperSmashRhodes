using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Runtime.Gauge;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.Character{
public class PlayerCharacter_Rosmontis : PlayerCharacter {
    public override float gravityScale {
        get {
            if (swordManager.floating && swordManager.belowFloatingHeight) return 0;
            return base.gravityScale;
        }
    }

    private Gauge_Rosmontis_SwordManager swordManager => GetComponent<Gauge_Rosmontis_SwordManager>();

    protected override void OnContact() {
        base.OnContact();
        swordManager.SetFloating(false);
    }
}
}
