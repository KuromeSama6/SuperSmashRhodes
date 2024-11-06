using System;
using Spine.Unity;
using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class CharacterAnimationController : MonoBehaviour {
    private static readonly int ANI_WALK_DIRECTION = Animator.StringToHash("WalkDirection");
    private static readonly int ANI_IS_WALKING = Animator.StringToHash("IsWalking");
    private static readonly int ANI_IS_DASHING = Animator.StringToHash("IsDashing");
    private static readonly int ANI_IS_CROUCHING = Animator.StringToHash("IsCrouching");
    
    private PlayerCharacter character;
    private Animator animator;
    private SkeletonMecanim skeleton;

    private float _walkDirection;
    
    private void Start() {
        character = GetComponent<PlayerCharacter>();
        animator = GetComponentInChildren<Animator>();
        skeleton = GetComponent<SkeletonMecanim>();
    }

    private void Update() {
        {
            // movement update
            var direction = character.moveDirection;
            _walkDirection = Mathf.Lerp(_walkDirection, direction, Time.deltaTime * 10f);
            
            animator.SetFloat(ANI_WALK_DIRECTION, _walkDirection);
            animator.SetBool(ANI_IS_WALKING, !Mathf.Approximately(direction, 0f));
            animator.SetBool(ANI_IS_DASHING, character.isDashing);
            animator.SetBool(ANI_IS_CROUCHING, character.isCrouching);
        }
    }
}
}
