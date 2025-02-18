using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class FrameDataRegister : RuntimeCharacterDataRegister {
    public int hitstunFrames { get => _hitstunFrames + carriedHitstunFrames; }
    public int blockstunFrames { get; set; }
    public int throwInvulnFrames { get; set; }
    public int landingRecoveryFrames { get; set; }
    public LandingRecoveryFlag landingFlag { get; set; }
    public List<Vector2> groundBounces { get; } = new();
    public List<Vector2> wallBounces { get; } = new();

    public bool shouldGroundBounce => groundBounces.Count > 0;
    public bool shouldWallBounce => wallBounces.Count > 0;
    
    private int _hitstunFrames;
    private int carriedHitstunFrames;

    public FrameDataRegister(PlayerCharacter owner) : base(owner) {

    }

    public void Tick() {
        if (carriedHitstunFrames > 0) {
            --carriedHitstunFrames;
        } else if (_hitstunFrames > 0) {
            --_hitstunFrames;
        }
        
        
        if (blockstunFrames > 0) {
            --blockstunFrames;
        }
        
        if (throwInvulnFrames > 0) {
            --throwInvulnFrames;
        }
        
        // if (landingRecoveryFrames > 0) {
        //     if (landingFlag.HasFlag(LandingRecoveryFlag.UNTIL_LAND) && !owner.airborne) {
        //         --landingRecoveryFrames;
        //     }
        // }
    }

    public void SetHitstunFrames(int frames, int carried) {
        _hitstunFrames = frames;
        
        // Carried hitstun frames are capped
        if (owner.comboCounter.count > 0) {
            // carriedHitstunFrames = Mathf.Max(carriedHitstunFrames + carried, 8);
        }
    }
    
    public void AddGroundBounce(Vector2 bounce) {
        groundBounces.Add(bounce);
    }

    public Vector2 ConsumeContactBounce() {
        if (groundBounces.Count == 0) {
            return Vector2.zero;
        }
        Vector2 bounce = groundBounces[0];
        groundBounces.RemoveAt(0);
        return bounce;
    }
    
    public void AddWallBounce(Vector2 bounce) {
        wallBounces.Add(bounce);
    }

    public Vector2 ConsumeWallBounce() {
        if (wallBounces.Count == 0) {
            return Vector2.zero;
        }
        Vector2 bounce = wallBounces[0];
        wallBounces.RemoveAt(0);
        return bounce;
    }
}

}
