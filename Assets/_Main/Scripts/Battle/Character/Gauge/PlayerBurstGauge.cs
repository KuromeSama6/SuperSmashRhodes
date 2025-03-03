using System;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Character.Gauge {
public class PlayerBurstGauge : CharacterComponent, IManualUpdate, IReflectionSerializable {
    public ClampedFloat gauge { get; } = new(0f, 620f, 300f);
    private List<BurstGaugeDelta> deltas { get; } = new();
    public bool burstAvailable { get; set; }
    public bool burstUsed { get; set; }
    public float releaseFrames { get; set; }

    public bool driveRelease { get; private set; }
    public int maxReleaseFrames => (int)(4 * 60f * 1.5f);
    
    public bool canBurst => burstAvailable && !burstUsed && !player.stateFlags.HasFlag(CharacterStateFlag.DISABLE_BURST);
    public bool canDriveRelease => gauge.value >= 500f && !driveRelease;
    
    public ReflectionSerializer reflectionSerializer { get; private set; }
    
    private void Start() {
        reflectionSerializer = new(this);
        gauge.value = 500f;
        burstAvailable = true;
        burstUsed = false;
    }

    public override void OnRoundInit() {
        base.OnRoundInit();
        burstUsed = false;
        burstAvailable = gauge.value >= 450f;
    }

    public void ManualUpdate() {
        
    }

    public void LogicUpdate() {
        if (GameManager.inst.globalStateFlags.HasFlag(CharacterStateFlag.PAUSE_GAUGE)) return;
         
        if (driveRelease) {
            releaseFrames -= 1;
            if (releaseFrames <= 0 && !(player.activeState is CharacterAttackStateBase)) {
                EndDriveRelease();
            }

        } else {
            {
                // passive gain
                var multiplier = 1f;
                if (player.healthPercent <= .25f) {
                    multiplier = 2.2f;
                } else if (player.healthPercent <= .5f) {
                    multiplier = 1.2f;
                }
                
                if (gauge.value < 500f) {
                    if (gauge.value >= 400f) {
                        AddDelta(1f / 60f * multiplier, 1);
                    } else if (gauge.value >= 200f) {
                        AddDelta(1.5f / 60f * multiplier, 1);
                    } else if (gauge.value >= 100f) {
                        AddDelta(2f / 60f * multiplier, 1);
                    } else {
                        AddDelta(3f / 60f * multiplier, 1);
                    }
                }
            }
        
            {
                // tick deltas
                foreach (var delta in deltas) {
                    gauge.value += delta.value;
                    delta.frames--;
                }
                deltas.RemoveAll(delta => delta.frames <= 0);
            }   
        }

        if (gauge.value >= 450f && !burstUsed) {
            if (!burstAvailable) {
                player.audioManager.PlaySound("cmn/battle/sfx/burst_alert/0");
            }
            burstAvailable = true;
        }
    }

    public void AddDelta(float amount, int frames) {
        if (amount == 0) return;
        deltas.Add(new(frames, amount));
    }
    
    public void AddDeltaTotal(float amount, int frames) {
        if (amount == 0) return;
        deltas.Add(new(frames, amount / frames));
    }

    public void BeginDriveRelease(int frames) {
        driveRelease = true;
        releaseFrames = frames;
        gauge.value = 0f; 
        
    }

    public void EndDriveRelease() {
        if (!driveRelease) return;
        driveRelease = false;
        player.audioManager.PlaySound("cmn/battle/sfx/driverelease_end");
    }
}

public class BurstGaugeDelta : IHandleSerializable {
    public int frames;
    public float value;
    
    public BurstGaugeDelta(int frames, float value) {
        this.frames = frames;
        this.value = value;
    }

    public IHandle GetHandle() {
        return new Handle(this);
    }
    
    private struct Handle : IHandle {
        private int frames;
        private float value;
        
        public Handle(BurstGaugeDelta delta) {
            frames = delta.frames;
            value = delta.value;
        }
        
        public object Resolve() {
            return new BurstGaugeDelta(frames, value);
        }
    }
}
}
