using System;
using System.Collections.Generic;
using SuperSmashRhodes.Battle.Serialization;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class FrameDataRegister : RuntimeCharacterDataRegister, IReflectionSerializable {
    public int hitstunFrames { get => _hitstunFrames + carriedHitstunFrames; }
    public int blockstunFrames { get; set; }
    public int throwInvulnFrames { get; set; }
    public int landingRecoveryFrames { get; set; }
    public LandingRecoveryFlag landingFlag { get; set; }
    [SerializationOptions(SerializationOption.EXCLUDE)]
    public List<BounceData> groundBounces { get; } = new();
    [SerializationOptions(SerializationOption.EXCLUDE)]
    public List<BounceData> wallBounces { get; } = new();

    public bool shouldGroundBounce => groundBounces.Count > 0;
    public bool shouldWallBounce => wallBounces.Count > 0;
    
    private int _hitstunFrames;
    private int carriedHitstunFrames;

    public ReflectionSerializer reflectionSerializer { get; }
    
    public FrameDataRegister(PlayerCharacter owner) : base(owner) {
        reflectionSerializer = new(this);
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
    
    public void AddGroundBounce(Vector2 bounce, BounceFlags flags = BounceFlags.NONE) {
        groundBounces.Add(new(bounce, flags));
    }
    public void AddGroundBounce(BounceData bounce) {
        groundBounces.Add(bounce);
    }

    public BounceData ConsumeContactBounce() {
        if (groundBounces.Count == 0) {
            return default;
        }
        var bounce = groundBounces[0];
        groundBounces.RemoveAt(0);
        return bounce;
    }
    
    public void AddWallBounce(BounceData bounce) {
        wallBounces.Add(bounce);
    }
    
    public void AddWallBounce(Vector2 bounce, BounceFlags flags = BounceFlags.NONE) {
        wallBounces.Add(new(bounce, flags));
    }

    public BounceData ConsumeWallBounce() {
        if (wallBounces.Count == 0) {
            return default;
        }
        var bounce = wallBounces[0];
        wallBounces.RemoveAt(0);
        return bounce;
    }

    public void Serialize(StateSerializer serializer) {
        reflectionSerializer.Serialize(serializer);
        
        serializer.PutList("groundBounces", groundBounces);
        serializer.PutList("wallBounces", wallBounces);
    }

    public void Deserialize(StateSerializer serializer) {
        reflectionSerializer.Deserialize(serializer);
        
        serializer.GetList("groundBounces", groundBounces);
        serializer.GetList("wallBounces", wallBounces);
    }
}

public struct BounceData {
    public readonly Vector2 bounceForce;
    public readonly BounceFlags flags;
    
    public BounceData(Vector2 force, BounceFlags flags = BounceFlags.NONE) {
        bounceForce = force;
        this.flags = flags;
    }
}

[Flags]
public enum BounceFlags {
    NONE = 0,
    HEAVY = 1 << 0,
}

}
