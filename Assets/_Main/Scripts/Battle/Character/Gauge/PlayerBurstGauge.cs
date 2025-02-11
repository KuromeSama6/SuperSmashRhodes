using System;
using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Character.Gauge {
public class PlayerBurstGauge : CharacterComponent {
    public ClampedFloat gauge { get; } = new(0f, 620f, 300f);
    private List<BurstGaugeDelta> deltas { get; } = new();
    public bool burstAvailable { get; private set; }
    public bool burstUsed { get; private set; }
    
    private void Start() {
        
    }

    public override void OnRoundInit() {
        base.OnRoundInit();
        gauge.value = 300f;

        burstAvailable = burstUsed = false;
    }

    private void FixedUpdate() {
        {
            // passive gain
            if (gauge.value >= 500f) {
                AddDelta(1f / 60f, 1);
            } else if (gauge.value >= 400f) {
                AddDelta(1.5f / 60f, 1);
            } else if (gauge.value >= 200f) {
                AddDelta(2f / 60f, 1);
            } else if (gauge.value >= 100f) {
                AddDelta(3f / 60f, 1);
            } else {
                AddDelta(4f / 60f, 1);
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
    
}

public class BurstGaugeDelta {
    public int frames;
    public float value;
    
    public BurstGaugeDelta(int frames, float value) {
        this.frames = frames;
        this.value = value;
    }
}
}
