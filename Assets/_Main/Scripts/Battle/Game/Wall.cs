using System;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.Game;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class Wall : MonoBehaviour {
    public EntitySide side;

    private void Start() {
        
    }

    private void Update() {
        
    }

    private void OnCollisionEnter2D(Collision2D other) {
        GameManager.inst.HandleWallCollision(this, other.gameObject.GetComponent<PlayerCharacter>(), true);
    }
    
    private void OnCollisionStay2D(Collision2D other) {
        GameManager.inst.HandleWallCollision(this, other.gameObject.GetComponent<PlayerCharacter>(), false);
    }

}
}
