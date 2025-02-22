using Spine.Unity;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class CinematicCharacterSocket : IHandleSerializable {
    private PlayerCharacter target;
    private PlayerCharacter owner;
    private string boneName;
    private Vector3 offset;
    
    private GameObject parentGameObject;
    private GameObject containerGameObject;
    public bool attached { get; private set; }
    private string parentName => $"Cinematic[{target.playerIndex} -> {boneName}@{owner.playerIndex}]";
    private string containerName => $"{parentName}$container";
    
    public CinematicCharacterSocket(PlayerCharacter target, PlayerCharacter owner, string boneName, Vector3 offset = default) {
        this.target = target;
        this.owner = owner;
        this.boneName = boneName;
        this.offset = offset;
    }

    public void Attach() {
        if (attached) return;

        parentGameObject = new GameObject(parentName);
        var follower = parentGameObject.AddComponent<BoneFollower>();

        follower.SkeletonRenderer = owner.animation.animation;
        follower.followBoneRotation = true;
        
        var suc = follower.SetBone(boneName);
        if (!suc) {
            Debug.LogError($"Failed to attach bone {boneName} to {owner.playerIndex}: bone not found");
            GameObject.Destroy(parentGameObject);
            parentGameObject = null;
            return;
        }

        containerGameObject = new GameObject(containerName);
        containerGameObject.transform.SetParent(parentGameObject.transform);
        containerGameObject.transform.localPosition = offset;
        
        // attach player
        target.transform.SetParent(containerGameObject.transform);
        target.rb.linearVelocity = Vector2.zero;
        target.transform.Reset();
        // Debug.Log($"target {target} pos {target.transform.localPosition}");

        attached = true;
    }

    public void Tick() {
        if (!attached) return;
        target.transform.localPosition = Vector3.zero - offset;
        target.transform.localEulerAngles = Vector3.zero;
        // Debug.Log(target.transform.position);
        // Debug.Log("tick");
    }
    
    public void Release() {
        if (!attached) return;
        attached = false;
        var pos = target.transform.position;
        var rot = target.transform.eulerAngles;
        target.transform.SetParent(null);

        // var ea = target.transform.localEulerAngles;
        // ea.y = 0;
        // ea.z = 0;
        target.transform.localEulerAngles = Vector3.zero;
        target.transform.position = pos + new Vector3(owner.side == EntitySide.LEFT ? 1 : -1, 0, 0);
        
        GameObject.Destroy(parentGameObject);
    }
    
    public IHandle GetHandle() {
        return new Handle(this);
    }
    
    private struct Handle : IHandle {
        private PlayerHandle target;
        private PlayerHandle owner;
        private string boneName;
        private Vector3 offset;
        private bool attached;
        private GameObjectHandle parentGameObject;
        private GameObjectHandle containerGameObject;
        
        public Handle(CinematicCharacterSocket socket) {
            target = new PlayerHandle(socket.target);
            owner = new PlayerHandle(socket.owner);
            boneName = socket.boneName;
            offset = socket.offset;
            attached = socket.attached;
            parentGameObject = new GameObjectHandle(socket.parentGameObject);
            containerGameObject = new GameObjectHandle(socket.containerGameObject);
        }
        
        public object Resolve() {
            var ret = new CinematicCharacterSocket((PlayerCharacter)target.Resolve(), (PlayerCharacter)owner.Resolve(), boneName, offset);
            if (attached) {
                ret.Attach();
            }
            return ret;
        }

        public override string ToString() {
            return $"CinematicCharacterSocket$Handle(target={target}, owner={owner}, boneName={boneName}, offset={offset}, attached={attached}, parentGameObject={parentGameObject}, containerGameObject={containerGameObject})";
        }
    }

}
}
