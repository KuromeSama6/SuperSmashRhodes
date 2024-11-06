using SuperSmashRhodes.FScript;
using SuperSmashRhodes.FScript.Input;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class PlayerCharacter : Entity {
    public float moveDirection { get; private set; }
    private PlayerInputModule inputModule;
    
    protected override void Start() {
        base.Start();
        inputModule = GetComponent<PlayerInputModule>();
    }

    protected override void Update() {
        base.Update();
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        
        UpdateMovement();

    }

    private void UpdateMovement() {
        InputChord input = inputModule.localBuffer.thisFrame;

        if (input.HasInput(InputType.FORWARD)) {
            moveDirection = 1f;
        } else if (input.HasInput(InputType.BACKWARD)) {
            moveDirection = -1f;
        } else {
            moveDirection = 0f;
            
        }

    }
}
}
