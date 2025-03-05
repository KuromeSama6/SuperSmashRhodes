using System;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Character.Gauge {
public class PlayerMeterGauge : CharacterComponent, IManualUpdate, IReflectionSerializable {
    public ClampedFloat gauge { get; } = new(0, 100f);
    public ClampedFloat balance { get; } = new(-250f, 250f);
    public int penaltyFrames { get; set; } = 0;

    public float meterGainMultiplier {
        get {
            float ret = 1f * meterBalanceMultiplier;

            if (player.opponent.comboCounter.inCombo) {
                // Debug.Log(player.comboDecayData.meterGainProrationCurve.Evaluate(player.opponent.comboCounter.comboDecay));
                ret *= player.comboDecayData.meterGainProrationCurve.Evaluate(player.opponent.comboCounter.comboDecay);
            }

            var distance = player.opponentDistance;
            if (distance >= 2f) ret *= 0.8f;
            else if (distance >= 4f) ret *= 0.6f;

            if (penaltyFrames > 0) ret *= .1f;
            
            return ret;
        }
    }

    public float meterBalanceMultiplier {
        get {
            var balance = this.balance.value;
            if (balance >= 200f) return 1.5f;
            if (balance >= 150f) return 1.3f;
            if (balance >= 100f) return 1.2f;
            if (balance >= 50f) return 1.1f;
            if (balance >= -50f) return 1f;
            if (balance >= -100f) return .9f;
            if (balance >= -150f) return .8f;
            if (balance >= -200f) return .7f;
            return .5f;
        }
    }

    public ReflectionSerializer reflectionSerializer { get; private set; }
    

    private void Start() {
        reflectionSerializer = new(this);
    }

    public void AddMeter(float amount) {
        if (GameManager.inst.globalStateFlags.HasFlag(CharacterStateFlag.PAUSE_GAUGE)) return;
        gauge.value += amount * meterGainMultiplier;
    }
    
    public override void OnRoundInit() {
        base.OnRoundInit();
        gauge.value = 0f;
        balance.value = 0f;
    }

    public void ManualUpdate() {
        
    }
    public void EngineUpdate() {
        // tension balance update
        if (GameManager.inst.globalStateFlags.HasFlag(CharacterStateFlag.PAUSE_GAUGE)) return;
        
        if (penaltyFrames > 0) {
            penaltyFrames--;
        }
        
        {
            var balance = this.balance.value;
            if (balance > 0 && balance <= 75f) this.balance.value -= .01f;
            else if (balance > 75f) this.balance.value -= .03f;
            else if (balance < 0) this.balance.value += .01f;
        }
    }

}
}
