using Spine.Unity;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class CinematicCharacterSocket {
    private PlayerCharacter target;
    private PlayerCharacter owner;
    private string boneName;
    private Vector3 offset;
    
    private GameObject parentGameObject;
    private GameObject containerGameObject;
    private bool attached;
    
    public CinematicCharacterSocket(PlayerCharacter target, PlayerCharacter owner, string boneName, Vector3 offset = default) {
        this.target = target;
        this.owner = owner;
        this.boneName = boneName;
        this.offset = offset;
    }

    public void Attach() {
        if (attached) return;

        parentGameObject = new GameObject($"Cinematic[{target.playerIndex} -> {boneName}@{owner.playerIndex}]");
        var follower = parentGameObject.AddComponent<BoneFollower>();

        follower.SkeletonRenderer = owner.animation.animation;
        var suc = follower.SetBone(boneName);
        if (!suc) {
            Debug.LogError($"Failed to attach bone {boneName} to {owner.playerIndex}: bone not found");
            GameObject.Destroy(parentGameObject);
            parentGameObject = null;
            return;
        }

        containerGameObject = new GameObject("container");
        containerGameObject.transform.SetParent(parentGameObject.transform);
        containerGameObject.transform.localPosition = offset;
        
        // attach player
        target.transform.SetParent(containerGameObject.transform);
        target.rb.linearVelocity = Vector2.zero;
        target.transform.Reset();

        attached = true;
    }

    public void Release() {
        var pos = parentGameObject.transform.position;
        target.transform.SetParent(null);

        var ea = target.transform.localEulerAngles;
        ea.z = 0;
        target.transform.localEulerAngles = ea;

        target.transform.position = pos;
        
        GameObject.Destroy(parentGameObject);
    }
    
}
}
