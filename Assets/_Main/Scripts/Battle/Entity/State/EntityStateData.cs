using System;
using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using Unity.VisualScripting;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State {
public class EntityStateData : IReflectionSerializable {
    private readonly Entity owner;
    public ReflectionSerializer reflectionSerializer { get; }

    // Cancel options
    /// <summary>
    /// States that can be canceled into from this state.
    /// </summary>
    [SerializationOptions(SerializationOption.EXCLUDE)]
    public List<EntityState> cancelOptions { get; } = new List<EntityState>();

    /// <summary>
    /// A flag that determines if the current state can be cancelled into a specific type of states.
    /// </summary>
    public EntityStateType cancelFlag;
    public Dictionary<string, object> carriedVariables { get; } = new();
    public bool disableSideSwap = false;
    public float gravityScale = 1;
    public BackgroundUIData backgroundUIData = BackgroundUIData.DEFAULT;
    public float targetFrameRate = 60f;
    public StateIndicatorFlag extraIndicatorFlag = StateIndicatorFlag.NONE;
    public EntityGhostFXData? ghostFXData;
    public CharacterRenderColorData renderColorData = new();
    public bool physicsPushboxDisabled = false;
    public bool maySwitchSides = false;
    public float midscreenWallDistanceModifier = 0;
    public CameraData cameraData = new();

    public bool shouldApplySlotNeutralPose {
        get => TryGetCarriedVariable("_applySlotNeutralPose", out bool value) && value;
        set => owner.SetCarriedStateVariable("_applySlotNeutralPose", null, value);
    }

    public EntityStateData(Entity owner) {
        this.owner = owner;
        reflectionSerializer = new(this);
    }
    
    public string carriedLandingAnimation {
        get => GetCarriedVariable<string>("_landingAnimation");
        set => owner.SetCarriedStateVariable("_landingAnimation", "CmnLandingRecovery", value);
    }

    public void ClearCancelOptions() {
        cancelFlag = 0;
        cancelOptions.Clear();
    }
    
    public T GetCarriedVariable<T>(string key, T def = default) {
        if (carriedVariables.TryGetValue(key, out var value)) {
            if (!(value is T)) {
                Debug.LogError($"Carried variable [{key}] type mismatch, expected {typeof(T)}, got {value.GetType()}");
                return def;
            }
            return (T)value;
        }
        
        // Debug.LogError($"Carried variable [{key}] not found");
        return def;
    }
    
    public bool TryGetCarriedVariable<T>(string key, out T value) {
        if (carriedVariables.TryGetValue(key, out var obj)) {
            if (obj is T t) {
                value = t;
                return true;
            }
        }
        value = default;
        return false;
    }
    
    public void Serialize(StateSerializer serializer) {
        reflectionSerializer.Serialize(serializer);
        serializer.Serialize("cancelOptions", cancelOptions.Select(c => c.id).ToList());

        var carriedVariables = new Dictionary<string, object>(this.carriedVariables);
        serializer.Serialize("carriedVariable", new DirectReferenceHandle(carriedVariables));

    }
    
    public void Deserialize(StateSerializer serializer) {
        reflectionSerializer.Deserialize(serializer);
        
        cancelOptions.Clear();
        cancelOptions.AddRange(serializer.Deserialize<List<string>>("cancelOptions").Select(c => owner.states[c]).ToList());
        
        carriedVariables.Clear();
        var carriedVariable = serializer.Deserialize<DirectReferenceHandle>("carriedVariable");
        carriedVariables.AddRange((Dictionary<string, object>)carriedVariable.GetObject());
    }
}

public struct CharacterRenderColorData {
    public float lerpSpeed;
    public Color white;
    public Color black;
    public Flag flags;
    
    public CharacterRenderColorData(float lerpSpeed = 100f, Color? white = null, Color? black = null, Flag flags = Flag.NONE) {
        this.lerpSpeed = lerpSpeed;
        this.white = white ?? Color.white;
        this.black = black ?? Color.black;
        this.flags = flags;
    }
    
    public static CharacterRenderColorData PAUSE => new(default, default, default, Flag.PAUSE);
    
    [Flags]
    public enum Flag {
        NONE = 0,
        PAUSE = 1 << 0,
        FLICKER = 1 << 1,
    }
}

public struct CameraData {
    public string focusBone;
    public float cameraFovModifier;
    public float cameraWeightModifier;
    
    public CameraData(string focusBone = "root", float cameraFovModifier = 0, float cameraWeightModifier = 1) {
        this.focusBone = focusBone;
        this.cameraFovModifier = cameraFovModifier;
        this.cameraWeightModifier = cameraWeightModifier;
    }
}
}
