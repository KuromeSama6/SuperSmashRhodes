using System;
using System.Collections;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Match.Player;
using SuperSmashRhodes.Network.RoomManagement;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Battle.Results {
public class BattleResultsUI : SingletonBehaviour<BattleResultsUI> {
    [Title("References")]
    public CanvasGroup canvasGroup;
    public GameObject badgePrefab;
    public RectTransform badgeContainer;
    public Image portrait;

    public bool visible { get; set; } = false;
    
    private void Start() {
        canvasGroup.alpha = 0f;
    }

    private void Update() {
        canvasGroup.alpha = visible ? 1 : 0;
        

    }

    public void ShowRoundEnd(Room room, Player winner) {
        visible = true;

        portrait.sprite = winner.character.superPortrait;
        
        StartCoroutine(ShowRoundEndCoroutine(room, winner));
    }

    private IEnumerator ShowRoundEndCoroutine(Room room, Player winner) {
        badgeContainer.gameObject.ClearChildren();
        
        // match results
        for (int i = 0; i < room.config.winRounds; i++) {
            Instantiate(badgePrefab, badgeContainer);
        }

        yield return new WaitForSeconds(.5f);
        
        var wins = room.results.FindAll(c => c.winner == winner);
        for (int i = 0; i < wins.Count; i++) {
            var badge = badgeContainer.GetChild(i).gameObject;
            badge.transform.Find("Image").GetComponent<Image>().color = wins[i].completionStatus == RoundCompletionStatus.PERFECT ? "01CCFD".HexToColor() : Color.white;
            badge.GetComponent<Animator>().SetTrigger("Play");
            
            yield return new WaitForSeconds(1f);
        }
        
    }

}
}
