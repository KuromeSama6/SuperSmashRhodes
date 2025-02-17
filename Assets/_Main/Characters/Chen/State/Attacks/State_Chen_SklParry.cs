using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.State {
[NamedToken("Chen_SklParry")]
public class State_Chen_SklParry : State_Common_Parry {
    public State_Chen_SklParry(Entity entity) : base(entity) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_SPECIAL;
    public override float inputPriority => 5f;
    protected override string mainAnimation => "chr/SklParry";
    public override AttackFrameData frameData => new() { startup = driveRelease ? 0 : 5, active = driveRelease ? 11 : 6, recovery = 32 };
    protected override EntityStateType commonCancelOptions => EntityStateType.NONE;
    protected override InputFrame[] requiredInput => new[] {
        new InputFrame(InputType.DOWN, InputFrameType.PRESSED),
        new InputFrame(InputType.BACKWARD, InputFrameType.PRESSED),
        new InputFrame(InputType.D, InputFrameType.PRESSED),
    };
    protected override int normalInputBufferLength => 10;
    protected override float inputMeter => 1f;

    protected override void OnStartup() {
        base.OnStartup();
        player.audioManager.PlaySound("chr/chen/battle/sfx/drive/parry");
    }

    protected override void OnParry(AttackData attack) {
        // Debug.Log("parry");
        player.audioManager.PlaySound("cmn/battle/sfx/generic/generic_parry_success");
        player.fxManager.PlayGameObjectFX("chr/chen/battle/fx/prefab/drive_parry_success", CharacterFXSocketType.WORLD_UNBOUND, player.transform.position);
        
        stateData.backgroundUIData.priority = 1;
        stateData.backgroundUIData.dimAlpha = .9f;
        stateData.backgroundUIData.dimSpeed = 40f;
        TimeManager.inst.Schedule(0, 15);
        TimeManager.inst.Queue(() => {
            stateData.backgroundUIData.dimAlpha = 0f;
            stateData.backgroundUIData.dimSpeed = 10f;
        });
        
        player.meter.AddMeter(15f);
        player.burst.AddDeltaTotal(50, 120);
        AddCancelOptions();
        AddCancelOption(EntityStateType.CHR_ATK_ALL);
    }

    protected override void OnTick() {
        base.OnTick();
        
        if (frame == (driveRelease ? 5 : 26)) AddCancelOptions();
    }

    private void AddCancelOptions() {
        
    }
}
}
