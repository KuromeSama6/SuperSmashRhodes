using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Network.RoomManagement;
using SuperSmashRhodes.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Battle {
public class BattleAnnouncerUI : SingletonBehaviour<BattleAnnouncerUI> {
    [Title("References")]
    public Animator roundStartBannerAnimator;
    public Animator roundEndBannerAnimator;
    public TMP_Text roundText;
    public TMP_Text roundEndText;
    public Image roundEndStripeTop, roundEndStripeBottom;
    public CanvasGroup transitionCover;
    public TMP_Text roundDescriptionText;
    
    public bool transitionCoverVisible { get; set; }

    private void Start() {
        
    }

    private void Update() {
        transitionCover.alpha = Mathf.Lerp(transitionCover.alpha, transitionCoverVisible ? 1 : 0, Time.deltaTime * 10);
        var room = RoomManager.current;
        
        if (room != null) {
            if (room.config.isTraining) {
                roundDescriptionText.text = "Training";
            } else {
                // round description
                var p1Wins = room.GetWinCount(0);
                var p2Wins = room.GetWinCount(1);

                if (p1Wins == room.config.winRounds - 1 || p2Wins == room.config.winRounds - 1) {
                    roundDescriptionText.text = p1Wins == p2Wins ? "Final Round" : "Match Point";
                } else {
                    roundDescriptionText.text = $"First to {room.config.winRounds}";
                }
            }
        }
    }

    public void Show(int roundNumber) {
        roundStartBannerAnimator.SetTrigger("Show");
        roundText.text = $"Round {roundNumber}";
    }

    public void EndRound(RoundCompletionStatus result, bool isGameEnd) {
        roundEndBannerAnimator.SetTrigger("Show");

        if (result == RoundCompletionStatus.PERFECT) {
            roundEndText.text = "Perfect!";
        } else if (isGameEnd) {
            roundEndText.text = "Game!";
            
        } else {
            roundEndText.text = "Knockout!";
        }

        {
            // gradient
            // VertexGradient gradient;
            // if (result == RoundCompletionStatus.PERFECT) gradient = new();
            // else gradient = new(Color.white, Color.white, "bcbcbc".HexToColor(), "bcbcbc".HexToColor());
        }

        var stripeColor = Color.white;
        roundEndStripeTop.color = roundEndStripeBottom.color = stripeColor;

    }
}
}
