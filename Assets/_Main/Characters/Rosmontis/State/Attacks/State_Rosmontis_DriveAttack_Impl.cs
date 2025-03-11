using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.Character {
[NamedToken("Rosmontis_NmlAtk5D")]
public class State_Rosmontis_NmlAtk5D : State_Rosmontis_DriveAttack {
    public State_Rosmontis_NmlAtk5D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_5D;
    public override float inputPriority => 3;
    protected override string mainAnimation => "cmn/NmlAtk5D";
    public override AttackFrameData frameData => new(15, 1, 32);
    protected override Vector2 summonOffset => new(1, 0);

    protected override string projectileState => "Token_Rosmontis_ThrownSword_DriveAttack5D";
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.D, InputFrameType.PRESSED)
    };
}

[NamedToken("Rosmontis_NmlAtk6D")]
public class State_Rosmontis_NmlAtk6D : State_Rosmontis_DriveAttack {
    public State_Rosmontis_NmlAtk6D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_6D;
    public override float inputPriority => 3.1f;
    protected override string mainAnimation => "cmn/NmlAtk5D";
    public override AttackFrameData frameData => new(15, 1, 32);
    protected override Vector2 summonOffset => new(.5f, 0.5f);
    
    protected override string projectileState => "Token_Rosmontis_ThrownSword_DriveAttack6D";
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.FORWARD, InputFrameType.HELD),
        new InputFrame(InputType.D, InputFrameType.PRESSED)
    };

    protected override void OnStateBegin() {
        base.OnStateBegin();
    }
}

[NamedToken("Rosmontis_NmlAtk2D")]
public class State_Rosmontis_NmlAtk2D : State_Rosmontis_DriveAttack {
    public State_Rosmontis_NmlAtk2D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_2D;
    public override float inputPriority => 3.1f;
    protected override string mainAnimation => "cmn/NmlAtk5D";
    public override AttackFrameData frameData => new(15, 1, 32);
    protected override Vector2 summonOffset => new(1f, 1f);
    
    protected override string projectileState => "Token_Rosmontis_ThrownSword_DriveAttack2D";
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.DOWN, InputFrameType.HELD),
        new InputFrame(InputType.D, InputFrameType.PRESSED)
    };

    protected override void OnStateBegin() {
        base.OnStateBegin();
    }
}

[NamedToken("Rosmontis_NmlAtk4D")]
public class State_Rosmontis_NmlAtk4D : State_Rosmontis_DriveAttack {
    public State_Rosmontis_NmlAtk4D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_4D;
    public override float inputPriority => 3.1f;
    protected override string mainAnimation => "cmn/NmlAtk5D";
    public override AttackFrameData frameData => new(15, 1, 32);
    protected override Vector2 summonOffset => new(5f, 3f);
    
    protected override string projectileState => "Token_Rosmontis_ThrownSword_DriveAttack4D";
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.BACKWARD, InputFrameType.HELD),
        new InputFrame(InputType.D, InputFrameType.PRESSED)
    };

    protected override void OnStateBegin() {
        base.OnStateBegin();
    }
}

[NamedToken("Rosmontis_NmlAtk8D")]
public class State_Rosmontis_NmlAtk8D : State_Rosmontis_DriveAttack {
    public State_Rosmontis_NmlAtk8D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_8D;
    public override LandingRecoveryFlag landingRecoveryFlag => LandingRecoveryFlag.NONE;
    protected override AttackAirOkType airOk => AttackAirOkType.AIR;
    public override float inputPriority => 3;
    protected override string mainAnimation => "cmn/NmlAtk8D";
    public override AttackFrameData frameData => new(13, 30, 6);
    protected override Vector2 summonOffset => new(1, 0f);

    protected override string projectileState => "Token_Rosmontis_ThrownSword_DriveAttack8D";
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.D, InputFrameType.PRESSED)
    };
}

[NamedToken("Rosmontis_NmlAtk82D")]
public class State_Rosmontis_NmlAtk82D : State_Rosmontis_DriveAttack {
    public State_Rosmontis_NmlAtk82D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_82D;
    public override LandingRecoveryFlag landingRecoveryFlag => LandingRecoveryFlag.NONE;
    protected override AttackAirOkType airOk => AttackAirOkType.AIR;
    public override float inputPriority => 3.1f;
    protected override string mainAnimation => "cmn/NmlAtk8D";
    public override AttackFrameData frameData => new(13, 30, 6);
    protected override Vector2 summonOffset => new(1, -.5f);

    protected override string projectileState => "Token_Rosmontis_ThrownSword_DriveAttack82D";
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.DOWN, InputFrameType.HELD),
        new InputFrame(InputType.D, InputFrameType.PRESSED)
    };
}

[NamedToken("Rosmontis_NmlAtk86D")]
public class State_Rosmontis_NmlAtk86D : State_Rosmontis_DriveAttack {
    public State_Rosmontis_NmlAtk86D(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_86D;
    protected override AttackAirOkType airOk => AttackAirOkType.AIR;
    public override LandingRecoveryFlag landingRecoveryFlag => LandingRecoveryFlag.NONE;
    public override float inputPriority => 3.1f;
    protected override string mainAnimation => "cmn/NmlAtk8D";
    public override AttackFrameData frameData => new(13, 30, 6);
    protected override Vector2 summonOffset => new(1.2f, 0f);

    protected override string projectileState => "Token_Rosmontis_ThrownSword_DriveAttack86D";
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.FORWARD, InputFrameType.HELD),
        new InputFrame(InputType.D, InputFrameType.PRESSED)
    };
}
}
