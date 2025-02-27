using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Room;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.CharacterSelect {
public class CharacterVSScreen : SingletonBehaviour<CharacterVSScreen> {
    [Title("References")]
    public CanvasGroup canvasGroup;
    public Animator animator;
    
    public bool show { get; set; }
    
    private void Start() {
        
    }

    private void Update() {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, show ? 1 : 0, Time.deltaTime * 15f);
        animator.SetBool("Show", show);
    }
    
    public void Show(PlayerMatchData player1, PlayerMatchData player2) {
        show = true;
    }
}
}
