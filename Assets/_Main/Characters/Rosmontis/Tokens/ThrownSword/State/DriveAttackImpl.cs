using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.Tokens.State {
[NamedToken("Token_Rosmontis_ThrownSword_DriveAttack5D")]
public class Token_Rosmontis_ThrownSword_DriveAttack5D : Token_Rosmontis_ThrownSword_DriveAttack {
    public Token_Rosmontis_ThrownSword_DriveAttack5D(Entity entity) : base(entity) { }
    protected override int lifetime => 15;
    protected override float initialAcceleration => 10;
    protected override float perTickAcceleration => 100;
    protected override Vector2 terminalVelocity => new(10000, 0);
    protected override float startingRotation => 0;

    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(3, airborne ? 5 : 0);
    }
}

[NamedToken("Token_Rosmontis_ThrownSword_DriveAttack6D")]
public class Token_Rosmontis_ThrownSword_DriveAttack6D : Token_Rosmontis_ThrownSword_DriveAttack {
    public Token_Rosmontis_ThrownSword_DriveAttack6D(Entity entity) : base(entity) { }
    protected override int lifetime => 15;
    protected override float initialAcceleration => 10;
    protected override float perTickAcceleration => 100;
    protected override Vector2 terminalVelocity => new(10000, 10000);
    protected override float startingRotation => 20;

    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(.7f, airborne ? 7 : 0);
    }
}

[NamedToken("Token_Rosmontis_ThrownSword_DriveAttack2D")]
public class Token_Rosmontis_ThrownSword_DriveAttack2D : Token_Rosmontis_ThrownSword_DriveAttack {
    public Token_Rosmontis_ThrownSword_DriveAttack2D(Entity entity) : base(entity) { }
    protected override int lifetime => 15;
    protected override float initialAcceleration => 10;
    protected override float perTickAcceleration => 50;
    protected override Vector2 terminalVelocity => new(10000, 10000);
    protected override float startingRotation => 45;

    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(.5f, 7f);
    }
}

[NamedToken("Token_Rosmontis_ThrownSword_DriveAttack4D")]
public class Token_Rosmontis_ThrownSword_DriveAttack4D : Token_Rosmontis_ThrownSword_DriveAttack {
    public Token_Rosmontis_ThrownSword_DriveAttack4D(Entity entity) : base(entity) { }
    protected override int lifetime => 15;
    protected override float initialAcceleration => 20;
    protected override float perTickAcceleration => 100;
    protected override Vector2 terminalVelocity => new(10000, 10000);
    protected override float startingRotation => 225;

    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(-2, airborne ? 5 : 0);
    }
}

[NamedToken("Token_Rosmontis_ThrownSword_DriveAttack8D")]
public class Token_Rosmontis_ThrownSword_DriveAttack8D : Token_Rosmontis_ThrownSword_DriveAttack {
    public Token_Rosmontis_ThrownSword_DriveAttack8D(Entity entity) : base(entity) { }
    protected override int lifetime => 20;
    protected override float initialAcceleration => 10;
    protected override float perTickAcceleration => 100;
    protected override Vector2 terminalVelocity => new(10000, 0);
    protected override float startingRotation => 0;

    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(blocked ? 1.5f : 0.5f, airborne && !blocked ? 6 : 0);
    }
}

[NamedToken("Token_Rosmontis_ThrownSword_DriveAttack82D")]
public class Token_Rosmontis_ThrownSword_DriveAttack82D : Token_Rosmontis_ThrownSword_DriveAttack {
    public Token_Rosmontis_ThrownSword_DriveAttack82D(Entity entity) : base(entity) { }
    protected override int lifetime => 20;
    protected override float initialAcceleration => 10;
    protected override float perTickAcceleration => 100;
    protected override Vector2 terminalVelocity => new(10000, 0);
    protected override float startingRotation => -30f;

    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(blocked ? 2f : 0.7f, airborne && !blocked ? 6 : 0);
    }
}

[NamedToken("Token_Rosmontis_ThrownSword_DriveAttack86D")]
public class Token_Rosmontis_ThrownSword_DriveAttack86D : Token_Rosmontis_ThrownSword_DriveAttack {
    public Token_Rosmontis_ThrownSword_DriveAttack86D(Entity entity) : base(entity) { }
    protected override int lifetime => 20;
    protected override float initialAcceleration => 10;
    protected override float perTickAcceleration => 100;
    protected override Vector2 terminalVelocity => new(10000, 0);
    protected override float startingRotation => -10f;

    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return new Vector2(blocked ? 1f : .3f, airborne && !blocked ? 9 : 0);
    }
}
}
