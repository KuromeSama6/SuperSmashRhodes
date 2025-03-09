using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Runtime.State;
using SuperSmashRhodes.Runtime.Tokens;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.Gauge {
public class Gauge_Rosmontis_SwordManager : CharacterComponent, IEngineUpdateListener {
    [Title("References")]
    public Bone bone;
    public List<GameObject> swordPrefabs = new();

    public ClampedFloat power { get; } = new(0, 5, 5);
    public List<Entity_Rosmontis_Sword> swords { get; } = new();
    public bool floating { get; set; }
    
    private int powerCooldown = 0;
    private float initialFloatingHeight;

    public float maxPower {
        get {
            var ret = power.max;
            if (floating) ret -= 1;
            return ret;
        }
    }
    public bool belowFloatingHeight => player.transform.position.y < initialFloatingHeight;
    
    private void Start() {
        
    }

    public void EngineUpdate() {
        if (!player || player.activeState == null) return;
        // Debug.Log(string.Join(", ", swords));

        if (powerCooldown > 0) {
            --powerCooldown;
        } else if (!GameManager.inst.globalStateFlags.HasFlag(CharacterStateFlag.PAUSE_GAUGE)) {
            // add power
            var amount = .0075f;
            if (power.value < 2) amount += .0075f;
            else if (power.value < 4) amount += .0035f;
            
            if (player.activeState.type == EntityStateType.CHR_HITSTUN) amount *= 0.5f;
            if (player.activeState.type == EntityStateType.CHR_BLOCKSTUN) amount *= 0.2f;

            if (player.burst.driveRelease) amount *= 2f;
            
            power.value += amount;
        }
        
        if (power.value >= maxPower) {
            power.value = maxPower;
        }

        if (floating && belowFloatingHeight && player.rb.linearVelocityY < 0) {
            player.rb.linearVelocityY = 0;
        }
    }

    public void SetFloating(bool floating) {
        if (this.floating == floating) return;
        this.floating = floating;

        if (floating) {
            UsePower(1);
            player.rb.linearVelocity = Vector2.zero;
            initialFloatingHeight = player.transform.position.y;
        }
    }

    public void UsePower(float amount) {
        power.value -= amount;
        if (!player.burst.driveRelease) {
            powerCooldown = 30;
        }
    }
    
    public override void OnRoundInit() {
        base.OnRoundInit();
        InstantiateSwords();
        power.value = power.max;
    }

    private void InstantiateSwords() {
        foreach (var sword in swords) {
            if (sword) GameManager.inst.UnregisterEntity(sword);
        }
        swords.Clear();
        
        // Instantiate swords
        for (int i = 0; i < 4; i++) {
            var prefab = swordPrefabs[i];
            var go = Instantiate(prefab);
            go.name = $"Rosmontis_Sword${i}";
            
            var sword = go.GetComponent<Entity_Rosmontis_Sword>();
            GameManager.inst.RegisterEntity(sword);
            
            player.summons.Add(sword);
            sword.Init(i, this);
            sword.OnRoundInit();
            sword.BeginLogic();
            
            swords.Add(sword);
        }
    }
}
}
