using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.Serialization;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SuperSmashRhodes.UI {
public class DebugStateDisplay : PerSideUIElement<DebugStateDisplay> {
    [Title("References")]
    public TMP_Text text;
    
    private StateSerializer ser;

    private void Start() {
        
    }

    private void Update() {
        if (!player || player.activeState == null) {
            text.text = $"No active state";
            return;
        }

        StringBuilder sb = new();
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha0)) {
            ser = new StateSerializer();
            Stopwatch sw = new();
            sw.Start();
            player.Serialize(ser);
            sw.Stop();
            
            sb.AppendLine("State Serialization Dump");
            sb.AppendLine($"Serialization took {sw.ElapsedMilliseconds}ms ({sw.ElapsedTicks} ticks)");
            sb.AppendLine("----");

            foreach (var key in ser.objects.Keys) {
                var obj = ser.objects[key];
                FormatSerialized(key, obj, sb);
            }
            
            var path = Path.Join(Application.persistentDataPath, "/debug/serialization_dump.txt");
            
            text.text = $"Serialized game state dumped to {path} (Serialized in {sw.ElapsedMilliseconds}ms)";
            text.fontSize = 20;
            text.fontStyle |= FontStyles.Bold;
            text.color = Color.green;
            
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, sb.ToString());
            
            return;
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.Pause)) {
            Debug.Break();
            return;
        }
        
        if (UnityEngine.Input.GetKey(KeyCode.Alpha0)) {
            return;
        } else {
            ser = null;
            text.color = Color.black;
            text.fontStyle &= ~FontStyles.Bold;
            text.fontSize = 30;
        }

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

    private void FormatSerialized(string key, object obj, StringBuilder sb, int level = 0) {
        for (int i = 0; i < level; i++) {
            sb.Append("    ");
        }

        if (obj is DirectReferenceHandle directReferenceHandle) {
            sb.Append($"{key}: <DirectRef> -> {directReferenceHandle.GetObject()}");
            
        } else if (obj is SerializedDictionary dict) {
            sb.AppendLine($"{key}: <Dict> ({dict.Count})");
            foreach (var dictKey in dict) {
                FormatSerialized(dictKey.Key, dictKey.Value, sb, level + 1);
            }
            
        } else if (obj is SerializedCollection list) {
            sb.AppendLine($"{key}: <List> ({list.Count})");
            int i = 0;
            foreach (var d in list) {
                FormatSerialized($"[{i}]", d, sb, level + 1);
                i++;
            }
            
        } else {
            sb.Append($"{key}: ({(obj == null ? "<null>" : obj.GetType())}) {obj}");
        }
        sb.Append("\n");
    }
}
}
