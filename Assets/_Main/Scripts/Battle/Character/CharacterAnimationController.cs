using System;
using Sirenix.OdinInspector;
using Spine.Unity;
using SuperSmashRhodes.Battle.Game;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
[Obsolete]
public class CharacterAnimationController : MonoBehaviour, IManualUpdate {
    private static readonly int ANI_WALK_DIRECTION = Animator.StringToHash("WalkDirection");
    private static readonly int ANI_IS_WALKING = Animator.StringToHash("IsWalking");
    private static readonly int ANI_IS_DASHING = Animator.StringToHash("IsDashing");
    private static readonly int ANI_IS_CROUCHING = Animator.StringToHash("IsCrouching");
    private static readonly int ANI_IS_GROUNDED = Animator.StringToHash("IsGrounded");
    private static readonly int ANI_Y_VELOCITY = Animator.StringToHash("VelocityY");
    private static readonly int ANI_MANAGED_LAYER = 1;

    [Title("References")]
    public AnimationClip managedPlaceholderClip;
    
    private PlayerCharacter character;
    private Animator animator;
    private SkeletonMecanim skeleton;
    private AnimatorOverrideController overrideController;
    
    private float _walkDirection;

    private AnimationClip currentManagedClip;
    private int managedAnimationFrames;
    
    private void Start() {
        character = GetComponent<PlayerCharacter>();
        animator = GetComponentInChildren<Animator>();
        skeleton = GetComponent<SkeletonMecanim>();
        
        overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;
        
        animator.SetLayerWeight(ANI_MANAGED_LAYER, 0);
    }

    private void Update() {
        {
            // movement update
            var direction = character.moveDirection;
            _walkDirection = Mathf.Lerp(_walkDirection, direction, Time.deltaTime * 20f);
            
            animator.SetFloat(ANI_WALK_DIRECTION, _walkDirection);
            animator.SetBool(ANI_IS_WALKING, !Mathf.Approximately(direction, 0f));
            animator.SetBool(ANI_IS_DASHING, character.isDashing);
            animator.SetBool(ANI_IS_CROUCHING, character.isCrouching);
        }

        {
            // air
            animator.SetBool(ANI_IS_GROUNDED, true); //TODO: Implement grounded check
            animator.SetFloat(ANI_Y_VELOCITY, character.rb.linearVelocityY);
        }

        {
            // managed animation
            
        }
    }
    

    public void ManualUpdate() {
        
    }
    public void ManualFixedUpdate() {
        if (managedAnimationFrames > 0) {
            --managedAnimationFrames;
            if (managedAnimationFrames == 0) {
                CancelManagedAnimation();
            }
        }
    }

    public void CancelManagedAnimation() {
        animator.SetLayerWeight(ANI_MANAGED_LAYER, 0);
        managedAnimationFrames = 0;

        overrideController[currentManagedClip] = managedPlaceholderClip;
    }
    
}
}
