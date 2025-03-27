using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Network.RoomManagement;
using UnityEngine;

namespace SuperSmashRhodes.UI.Battle {
public class FrameDataDisplay : SingletonBehaviour<FrameDataDisplay> {
    [Title("References")]
    public CanvasGroup canvasGroup;
    public RectTransform container;
    public FrameDataBar[] bars;

    private GameManager gm => GameManager.inst;
    
    private void Start() {
        
    }

    private void Update() {
        if (!gm) return;
        var room = RoomManager.current;
        var visible = room == null || room.config.isTraining;
        canvasGroup.alpha = visible ? 1 : 0;
    }
}
}
