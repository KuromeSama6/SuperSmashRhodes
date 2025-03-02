using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Network.RoomManagement;
using SuperSmashRhodes.UI.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Battle {
public class BattleTimer : SingletonBehaviour<BattleTimer> {
    [Title("References")]
    public RotaryCounter counter;
    public Image infiniteTimeIndicator;

    private void Start() {
        counter.gameObject.SetActive(false);
        infiniteTimeIndicator.gameObject.SetActive(false);
    }

    private void Update() {
        var room = RoomManager.current;
        if (room == null || !room.config) return;

        infiniteTimeIndicator.gameObject.SetActive(room.config.infiniteTime);
        counter.gameObject.SetActive(!room.config.infiniteTime);

        if (!room.config.infiniteTime) {
            counter.target = Mathf.Round(RoomManager.current.time);
        }
        
    }
}
}
