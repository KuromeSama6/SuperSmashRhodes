using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sirenix.OdinInspector;
using Spine.Unity;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class EntityBoundingBoxManager : MonoBehaviour, IAutoSerialize {
    [Title("Configuration")]
    public bool createPushbox = true;
    public int hitboxCount = 0;
    public int hurtboxCount = 0;
    public SkeletonAnimation skeleton;
    public List<ExplicitBoundingBox> explicitBoundingBoxes = new();

    [Title("References")]
    public Entity entity;
        
    public EntityBoundingBox pushbox { get; private set; }
    private List<IEntityBoundingBox> boxes = new();

    private void Start() {
        
        // register
        // main pushbox
        if (skeleton) {
            pushbox = CreateBoundingBox("pushbox", BoundingBoxType.CHR_MAIN_PUSHBOX);
            for (int i = 0; i < hitboxCount; i++) CreateBoundingBox($"hb_{i}", BoundingBoxType.HITBOX);
            for (int i = 0; i < hurtboxCount; i++) CreateBoundingBox($"ub_{i}", BoundingBoxType.HURTBOX);   
        }
        
        // Debug.Log($"{boxes.Count} {explicitBoundingBoxes.Count}");
        boxes.AddRange(explicitBoundingBoxes);
        
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    private void Update() {
        // Debug.Log($"{entity.name}: pushbox={pushbox.box}");
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

    public void SetAll(bool enabled) {
        boxes.RemoveAll(c => c is Component component && !component);
        
        foreach (var box in boxes) {
            if (box != pushbox) {
                if (box == null || box.box == null) continue;
                box.box.enabled = enabled;
            }
        }
    }
    
    public void Serialize(StateSerializer serializer) {
        
    }
    public void Deserialize(StateSerializer serializer) {
        boxes.ForEach(c => c.EnsureAttachment());
    }
    
}
}
