using System;
using Sirenix.OdinInspector;
using Spine.Unity.Examples;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class EntityGhostFXManager : MonoBehaviour {
    private SkeletonGhost ghost;
    private Entity entity;

    public EntityGhostFXData? ghostFXData {
        get {
            if (entity.activeState == null) return null;
            
            if (entity.activeState.stateData.ghostFXData.HasValue) {
                return entity.activeState.stateData.ghostFXData;
            }

            if (entity is PlayerCharacter player && player.burst.driveRelease) {
                return new("D94300".HexToColor());
            }

            return null;
        }
    }
    
    private void Start() {
        entity = GetComponent<Entity>();
        ghost = GetComponentInChildren<SkeletonGhost>();
        
        if (!ghost) {
            Destroy(this);
            return;
        }
        ghost.ghostingEnabled = false;
    }

    private void Update() {
        var data = ghostFXData;
        if (data != null) {
            ghost.ghostingEnabled = true;
            ghost.color = data.Value.color;
            ghost.spawnInterval = data.Value.spawnInterval;
            ghost.maximumGhosts = data.Value.maxGhosts;
            ghost.fadeSpeed = data.Value.fadeSpeed;
            
        } else {
            ghost.ghostingEnabled = false;
        }
    }
}

public struct EntityGhostFXData {
    public Color color;
    public float spawnInterval;
    public int maxGhosts;
    public float fadeSpeed;

    public EntityGhostFXData(Color color, float spawnInterval = 0.03333334f, int maxGhosts = 10, float fadeSpeed = 10f) {
        this.color = color;
        this.spawnInterval = spawnInterval;
        this.maxGhosts = maxGhosts;
        this.fadeSpeed = fadeSpeed;
    }

}
}
