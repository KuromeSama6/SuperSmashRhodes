using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Network.RoomManagement;
using UnityEngine;
using UnityEngine.Events;

namespace SuperSmashRhodes.Battle {
public class ComboCounter : RuntimeCharacterDataRegister, IReflectionSerializable {
    public int displayedCount { get; private set; }
    public int count { get; private set; }
    public bool inCombo => count >= 1;
    
    private float overallProration = 1f;
    private float appliedProration = 1f;
    public float comboDecay { get; set; }
    private Dictionary<string, int> movesUsed = new();
    
    private string lastMove;
    private int multihitCount;
    private float multihitProration = 1f;
    
    public ReflectionSerializer reflectionSerializer { get; }
    
    public float finalScale {
        get {
            var ret = 1f;
            ret *= multihitProration;
            if (count <= 1) return ret;

            ret *= overallProration * appliedProration;
            return ret;
        }
    }
    
    public ComboCounter(PlayerCharacter owner) : base(owner) {
        reflectionSerializer = new(this);
    }

    public void RegisterAttack(
        float damage,
        IAttack move, 
        Entity victim, 
        DamageProperties flags,
        float multiplier = 1f, 
        bool blocked = false
    ) 
    {
        // Debug.Log(blocked);
        
        if (!blocked) ++displayedCount;
        if (flags.HasFlag(DamageProperties.SKIP_REGISTER)) return;
        // Debug.Log(flags.HasFlag(DamageProperties.MULTIHIT));
        var multihit = flags.HasFlag(DamageProperties.MULTIHIT);
        bool partOfMultihit = multihit && lastMove != null && lastMove == move.id;
        
        if (partOfMultihit) {
            multihitCount += 1;
            multihitProration *= move.GetComboProration(victim);
            // Debug.Log($"{move.GetComboProration(victim)} {multihitProration}");
            
        } else {
            multihitCount = 0;
            multihitProration = 1f;
            
            owner.burst.AddDeltaTotal(damage * .05f, 120);
            owner.meter.AddMeter(1.5f);
        }
        
        if (partOfMultihit) {
            return;
        }
        
        if (count == 0) {
            overallProration = move.GetFirstHitProration(victim);
        }
        count += 1;

        appliedProration *= move.GetComboProration(victim);
        
        // same move penalty
        var driveReleaseMultiplier = owner.burst.driveRelease ? .2f : 1f;
        
        if (movesUsed.ContainsKey(move.id)) {
            ++movesUsed[move.id];
            comboDecay += 0.4f * multiplier * driveReleaseMultiplier;

        } else {
            movesUsed[move.id] = 1;
        } 

        // combo decay
        // Debug.Log($"add decay, {countSameMove} {movesUsed.ContainsKey(move.id)}");
        var amount = move.GetComboDecay(victim);
        if (victim is PlayerCharacter player) {
            if (!player.airborne) amount *= .5f;
            else amount *= 0.8f;
        }
        comboDecay += amount * multiplier * driveReleaseMultiplier;
        
        lastMove = move.id;
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
        lastMove = null;

        if (RoomManager.current.config.isTraining) {
            owner.health = owner.config.health;
        }

        // Debug.Log($"{owner} combo end");
    }
    
}
}
