using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Runtime.Gauge;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Exusiai_SklRoll_FThrow")]
public class State_Exusiai_SklRoll_FThrow : State_Common_CommandThrow {
    public State_Exusiai_SklRoll_FThrow(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_SPECIAL_TRIGGER;
    public override float inputPriority => 5f;
    protected override string mainAnimation => "chr/SklRoll_FThrow";
    protected override InputFrame[] requiredInput => new[] { new InputFrame(InputType.FORWARD, InputFrameType.HELD), new InputFrame(InputType.HS, InputFrameType.PRESSED)};

    public override AttackFrameData frameData => new AttackFrameData() {
        startup = 7,
        active = 4,
        recovery = 30
    };
    protected override string whiffAnimation => "chr/SklRoll_FThrow_W";
    protected override float distanceRequirement => 1.3f;
    protected override int animationLength => 74;

    protected override void OnThrowWhiff(PlayerCharacter other) {
        base.OnThrowWhiff(other);
        player.ApplyForwardVelocity(new(8f, 0));
    }

    protected override void OnFinalHit() {
        base.OnFinalHit();
        player.GetComponent<Gauge_Exusiai_AmmoGauge>().AddMagazine();
        player.meter.AddMeter(5);
    }
}
}
