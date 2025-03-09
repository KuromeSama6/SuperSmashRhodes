using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Runtime.Gauge;

namespace SuperSmashRhodes.Runtime.State{
[NamedToken("Rosmontis_ChrFloatStart")]
public class ChrFloatStart : CharacterState {
    public override EntityStateType type => EntityStateType.CHR_ATK_SPECIAL_TRIGGER;
    public override float inputPriority => 2f;
    public override bool alwaysCancellable => true;

    public override bool mayEnterState {
        get {
            if (player.airOptions <= 0) return false;
            if (!(player.activeState is State_CmnAirNeutral)) return false;
            var gauge = player.GetComponent<Gauge_Rosmontis_SwordManager>();
            if (gauge.floating) return false;
            if (gauge.power.value < 1) return false;
            return true;
        }
    }
    
    public ChrFloatStart(Entity entity) : base(entity) {
        
    }

    protected override void OnStateBegin() {
        base.OnStateBegin();
        player.airOptions = 0;
        player.animation.AddUnmanagedAnimation("chr/ChrFloatStart", false);
        
        player.GetComponent<Gauge_Rosmontis_SwordManager>().SetFloating(true);
    }

    public override EntityStateSubroutine BeginMainSubroutine() {
        return c => c.Exit(13);
    }
    
    public override bool IsInputValid(InputBuffer buffer) {
        return buffer.TimeSlice(5).ScanForInput(entity.side, new InputFrame(InputType.UP, InputFrameType.PRESSED));
    }
}
}
