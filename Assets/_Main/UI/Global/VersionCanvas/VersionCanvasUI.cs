
using System;
using System.Collections;
using System.Text;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Config.Global;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Network;
using SuperSmashRhodes.Network.RoomManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Global.VersionCanvas {
public class VersionCanvasUI : SingletonBehaviour<VersionCanvasUI> {
    [Title("References")]
    public Image networkIcon;
    public TMP_Text versionText;
    public RectTransform container;
    
    private void Start() {
    }

    private void Update() {
        var sb = new StringBuilder();
        var settings = ApplicationGlobalSettings.inst;
        sb.AppendLine($"{settings.branch}-{settings.semver}-b{settings.buildNumber}");
        
        // room
        {
            var room = RoomManager.current;
            if (room != null) {
                if (room is NetworkRoom networkRoom) {
                    sb.AppendLine(networkRoom.session.config.userId);
                    sb.AppendLine($"RTA {networkRoom.session.lastPingLatency}ms");

                    if (networkRoom.fighting) {
                        var inputManager = networkRoom.inputManager;
                        sb.AppendLine($"DL Send {inputManager.sendFrame} Receive {inputManager.receiveFrame}");
                    }

                } else {
                    sb.AppendLine("local");
                }
            }
            
            var str = sb.ToString();
            if (versionText.text != str) {
                versionText.text = sb.ToString();
                LayoutRebuilder.ForceRebuildLayoutImmediate(container);
            }
        }
        
    }

}
}
