using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Runtime.Gauge;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.Tokens {
public class Entity_Rosmontis_Sword : Entity, IEngineUpdateListener {
    [Title("References")]
    public BoneFollower boneFollower;
    public SkeletonAnimation skeleton;
    public List<String> slotNames = new();
    
    public int index { get; private set; }
    public Gauge_Rosmontis_SwordManager manager { get; private set; }

    private Bone targetBone;
    private MeshRenderer meshRenderer;
    
    private SkeletonAnimation playerSkeleton => owner.animation.animation;
    public override bool shouldSimulatePhysics => false;
    /**
     * Sword slot to target slot.
     */
    private readonly Dictionary<Slot, Slot> slots = new();

    protected override void Start() {
        base.Start();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Init(int index, Gauge_Rosmontis_SwordManager manager) {
        this.index = index;
        this.manager = manager;
        owner = manager.player;

        var boneName = $"F_Weapon_{index + 1}";
        targetBone = playerSkeleton.skeleton.FindBone(boneName);
        if (targetBone == null)
            throw new ArgumentException($"Target bone not found for Rosmontis' sword: {boneName}");

        boneFollower.SkeletonRenderer = playerSkeleton;
        boneFollower.bone = targetBone;
        
        // find slots
        foreach (string name in slotNames) {
            var targetSlot = playerSkeleton.skeleton.FindSlot(name);
            var swordSlot = skeleton.skeleton.FindSlot(name);
            if (targetSlot == null || swordSlot == null)
                throw new ArgumentException($"Slot not found for Rosmontis' sword: {name}");
            slots[swordSlot] = targetSlot;
        }


        // Debug.Log($"Rosmontis sword init: index: {index}, bone: {targetBone}, slots: {string.Join(",", slots.Keys)}");
    }
    
    
    public override void OnRoundInit() {
        base.OnRoundInit();
    }

    public override void BeginLogic() {
        base.BeginLogic();
    }

    public override void EngineUpdate() {
        base.EngineUpdate();
        
        transform.position = new(transform.position.x, transform.position.y, owner.transform.position.z);
        foreach (var (current, target) in slots) {
            target.A = 0;
            current.Attachment = target.Attachment;
        }

        if (index == 1) {
            meshRenderer.sortingOrder = 0;
        } else {
            meshRenderer.sortingOrder = owner.skeletonSortingOrder + index switch {
                0 => 5,
                2 => -4,
                3 => -5,
                _ => 0
            };   
        }
    }

    public override EntityState GetDefaultState() {
        return states["Token_Rosmontis_Sword_Default"];
    }
}
}
