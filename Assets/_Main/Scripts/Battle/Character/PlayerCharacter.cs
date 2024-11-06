using SuperSmashRhodes.FScript;
using SuperSmashRhodes.FScript.Input;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class PlayerCharacter : Entity {
    public float moveDirection { get; private set; }
    public bool isDashing { get; private set; }
    public bool isCrouching { get; private set; }
    public int framesDashed { get; private set; }
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
        
        // movement
        {
            isDashing = input.HasInput(InputType.DASH);
            if (isDashing) ++framesDashed;
            else framesDashed = 0;

            isCrouching = input.HasInput(InputType.DOWN);
            if (!isCrouching) {
                if (input.HasInput(InputType.FORWARD)) {
                    moveDirection = 1f;
            
                } else if (input.HasInput(InputType.BACKWARD)) {
                    moveDirection = -1f;
                    if (isDashing) {
                        moveDirection = 0f;
                        isDashing = false;
                        //TODO: Backdash
                    }
            
                } else {
                    moveDirection = 0f;
                }   
            } else {
                moveDirection = 0f;
            }

            if (!Mathf.Approximately(moveDirection, 0f)) {
                // All characters have universal walking acceleration
                float force = isDashing ? config.dashAccelCurve.Evaluate(framesDashed) : 20f;
                AddContinuousForce(new Vector2(PhysicsUtil.NormalizeRelativeDirecionalForce(force * moveDirection, facing), 0f));
            }
        }
        
    }

    public override float managedXVelocityLimit {
        get {
            if (isPhysicallyMoving) {
                if (isDashing) return config.dashSpeed;
                return moveDirection < 0f ? config.backwalkSpeed : config.walkSpeed;
            }
            return -1;
        }
    }
}
}
