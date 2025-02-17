using System;
using System.Text;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace SuperSmashRhodes.UI {
public class DebugStateDisplay : PerSideUIElement<DebugStateDisplay> {
    [Title("References")]
    public TMP_Text text;

    private void Start() {
        
    }

    private void Update() {
        if (!player || player.activeState == null) {
            text.text = $"No active state";
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine($"State Debug P{player.playerIndex} eid {player.entityId}");
        sb.AppendLine("--- Position ---");
        sb.AppendLine($"X {player.transform.position.x:F2} Y {player.transform.position.y:F3} WallDist {player.wallDistance:F3} LW {player.pushboxManager.atLeftWall} RW {player.pushboxManager.atRightWall}");
        sb.AppendLine($"--- Combo Counter ---");
        var comboCounter = player.opponent.comboCounter;
        sb.AppendLine($"Decay {comboCounter.comboDecay}");
        sb.AppendLine($"--- Active State {player.activeState.id} @ {player.activeState} ---");
        sb.AppendLine($"--- Main Routine ---");
        sb.AppendLine($"Routines {player.activeState.activeRoutines} Frame {player.activeState.frame} Ani Frame {(int)(player.animation.animation.state.GetCurrent(0).AnimationTime / Time.fixedDeltaTime)}");
        sb.AppendLine($"--- Subroutine Stack ---");
        foreach (var routine in player.activeState.routines) {
            sb.AppendLine($"[SUB] {routine.enumerator} Ptr->{routine.parentFrame} Flags {routine.flags}");
        }
        
        text.text = sb.ToString();
    }
}
}
