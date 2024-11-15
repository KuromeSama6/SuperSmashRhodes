using System;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Character.Gauge {
public class PlayerMeterGauge : CharacterComponent {
    public ClampedFloat meter { get; } = new(0, 100f);
    public ClampedFloat meterBalance { get; } = new(-250f, 250f);

    public float meterGainMultiplier {
        get {
            float ret = 1f * meterBalanceMultiplier;
            
            if (player.opponent.comboCounter.inCombo) ret *= player.comboDecayData.meterGainProrationCurve.Evaluate(player.opponent.comboCounter.comboDecay);

            var distance = player.opponentDistance;
            if (distance >= 2f) ret *= 0.8f;
            else if (distance >= 4f) ret *= 0.6f;
            
            return ret;
        }
    }

    public float meterBalanceMultiplier {
        get {
            var balance = meterBalance.value;
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
    
    public override void OnRoundInit() {
        base.OnRoundInit();
        meter.value = 0f;
        meterBalance.value = 0f;
    }

    private void FixedUpdate() {
        // tension balance update
        {
            var balance = meterBalance.value;
            if (balance > 0 && balance <= 75f) meterBalance.value -= .01f;
            else if (balance > 75f) meterBalance.value -= .03f;
            else if (balance < 0) meterBalance.value += .01f;
        }
    }

}
}
