using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Battle.State.Implementation;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SuperSmashRhodes.UI {
public class DebugStateDisplay : PerSideUIElement<DebugStateDisplay> {
    [Title("References")]
    public TMP_Text text;
    
    private SerializedEngineState gameState;

    private void Start() {
        
    }

    private void Update() {
        if (!player || player.activeState == null) {
            text.text = $"No active state";
            return;
        }

        StringBuilder sb = new();
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha0)) {
            var path = Path.Join(Application.persistentDataPath, "/debug/serialization_dump.txt");
            text.fontSize = 20;
            text.fontStyle |= FontStyles.Bold;
            text.color = Color.green;
            
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            FightEngine.inst.QueueSaveGameState(state => {
                gameState = state;
                File.WriteAllText(path, state.DumpToString());
            });
            return;
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha9)) {
            // Debug.Log("Deserializing");
            if (gameState == null) {
                Debug.LogWarning("No serialized state to deserialize");
            } else {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                FightEngine.inst.QueueLoadGameState(gameState);
                stopwatch.Stop();
                // Debug.Log($"Operation completed in {stopwatch.ElapsedMilliseconds}ms ({stopwatch.ElapsedTicks} ticks)");
                
                var path = Path.Join(Application.persistentDataPath, "/debug/deserialization_dump.txt");
                // File.WriteAllText(path, GameStateManager.inst.SerializeGameState().DumpToString());
                FightEngine.inst.QueueSaveGameState(state => {
                    File.WriteAllText(path, state.DumpToString());
                });
            }
        }
        
        if (UnityEngine.Input.GetKeyDown(KeyCode.Pause)) {
            Debug.Break();
            return;
        }
        
        if (UnityEngine.Input.GetKey(KeyCode.Alpha0)) {
            return;
        } else {
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
        sb.AppendLine($"Frame {player.activeState.frame} Ani Frame {(int)(player.animation.animation.state.GetCurrent(0).AnimationTime / Time.fixedDeltaTime)}");
        sb.AppendLine($"Int {player.activeState.interruptFrames}");

        if (player.activeState is CharacterAttackStateBase attack) {
            sb.AppendLine($"Attack phase {attack.phase}, stage {attack.attackStage}, hits left {attack.hitsRemaining}, lock {attack.hitLock}");
        }
        
        text.text = sb.ToString();
    }
}
}
