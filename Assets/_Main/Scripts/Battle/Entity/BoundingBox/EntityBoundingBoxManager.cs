using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sirenix.OdinInspector;
using Spine.Unity;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class EntityBoundingBoxManager : MonoBehaviour {
    [Title("Configuration")]
    public int hitboxCount = 0;
    public int hurtboxCount = 0;
    public SkeletonAnimation skeleton;
    public List<IEntityBoundingBox> explicitBoundingBoxes = new();
    
    public EntityBoundingBox pushbox { get; private set; }
    private Entity entity;
    private List<IEntityBoundingBox> boxes = new();

    private void Start() {
        entity = GetComponentInParent<Entity>();
        
        // register
        // main pushbox
        pushbox = CreateBoundingBox("pushbox", BoundingBoxType.CHR_MAIN_PUSHBOX);
        for (int i = 0; i < hitboxCount; i++) CreateBoundingBox($"hb_{i}", BoundingBoxType.HITBOX);
        for (int i = 0; i < hurtboxCount; i++) CreateBoundingBox($"ub_{i}", BoundingBoxType.HURTBOX);
        boxes.AddRange(explicitBoundingBoxes);
    }

    private EntityBoundingBox CreateBoundingBox(string name, BoundingBoxType type) {
        if (skeleton.skeleton.FindSlot(name) == null)
            throw new ArgumentException($"No such bounding box: {name}");
        
        var go = new GameObject();
        go.layer = LayerMask.NameToLayer("BoundingBox");
        go.transform.parent = transform;
        go.transform.Reset();
        go.name = $"{entity.name}-{name}";
        
        // add scripts
        var follower = go.AddComponent<BoundingBoxFollower>();
        follower.skeletonRenderer = skeleton;
        follower.slotName = name;

        var bb = go.AddComponent<EntityBoundingBox>();
        bb.entity = entity;
        bb.owningPlayer = entity.owner;
        bb.type = type;
        boxes.Add(bb);

        return bb;
    }

    public void DisableAll() {
        foreach (var box in boxes) {
            if (box != pushbox) box.box.enabled = false;
        }
    }
    
}
}
