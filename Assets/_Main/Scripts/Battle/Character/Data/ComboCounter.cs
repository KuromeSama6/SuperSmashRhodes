using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.Battle.State.Implementation;
using UnityEngine;
using UnityEngine.Events;

namespace SuperSmashRhodes.Battle {
public class ComboCounter : RuntimeCharacterDataRegister {
    public int displayedCount { get; private set; }
    public int count { get; private set; }
    public bool inCombo => count >= 2;
    
    private float overallProration = 1f;
    private float appliedProration = 1f;
    public float comboDecay { get; set; }
    private Dictionary<string, int> movesUsed = new();

    public float finalScale {
        get {
            if (count <= 1) return 1f;
            return overallProration * appliedProration;
        }
    }
    
    public ComboCounter(PlayerCharacter owner) : base(owner){

    }

    public void RegisterAttack(IAttack move, Entity victim, bool skipRegister = false, float multiplier = 1f, bool countSameMove = true) {
        if (skipRegister) return;
        ++displayedCount;
        
        if (count == 0) {
            overallProration = move.GetFirstHitProration(victim);
        }
        count += 1;

        appliedProration *= move.GetComboProration(victim);
        
        // same move penalty
        if (countSameMove) {
            if (movesUsed.ContainsKey(move.id)) {
                ++movesUsed[move.id];
                comboDecay += 0.4f * (movesUsed[move.id] + 1) * multiplier;

            } else movesUsed[move.id] = 1;   
        }

        {
            // combo decay
            var amount = move.GetComboDecay(victim);
            if (victim is PlayerCharacter player) {
                if (!player.airborne) amount *= .1f;
            }

            comboDecay += amount * multiplier;
        }
    }

    public float GetMoveSpecificProration(IAttack move) {
        float ret = 1f;
        var id = move.id;
        if (movesUsed.ContainsKey(id)) {
            ret -= Mathf.Clamp((movesUsed[id] - 1) * 0.09f, 0f, 0.5f);
        }
        return ret;
    }
    
    public void Reset() {
        count = displayedCount = 0;
        overallProration = 1f;
        appliedProration = 1f;
        comboDecay = 0;
        movesUsed.Clear();
        // Debug.Log($"{owner} combo end");
    }
    
}
}
