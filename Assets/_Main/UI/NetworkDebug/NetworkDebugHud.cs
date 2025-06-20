using Sirenix.OdinInspector;
using SuperSmashRhodes.Config.Global;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Network;
using SuperSmashRhodes.Network.Rollbit;
using SuperSmashRhodes.Network.RoomManagement;
using SuperSmashRhodes.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes {
public class NetworkDebugUI : MonoBehaviour {
    [Title("References")]
    [Title("Connection Info")]
    public TMP_InputField hostInput;
    public TMP_InputField portInput;
    public Button connectButton;
    public Button stunTestButton;
    public RollbitClientConfiguration clientConfiguration;
    public RoomConfiguration roomConfiguration;
    
    public TMP_Text buttonText;
    public TMP_Text statusText;

    [Title("Room Info")]
    public GameObject roomInfoPanel;
    public TMP_Text roomStatusText, playerCountText, spectatorCountText;
    public GameObject matchAcceptPanel;
    public TMP_Text matchAcceptText;
    public Button acceptMatchButton, declineMatchButton;
    
    private NetworkSession networkSession => NetworkManager.inst.currentSession;
    
    private void Start() {
        connectButton.onClick.AddListener(OnButtonPress);
        acceptMatchButton.onClick.AddListener(() => AcceptMatch(true));
        declineMatchButton.onClick.AddListener(() => AcceptMatch(false));
        stunTestButton.onClick.AddListener(TestStun);

        if (clientConfiguration) {
            hostInput.text = clientConfiguration.host;
            portInput.text = clientConfiguration.port.ToString();
        }
    }

    private void Update() {
        connectButton.gameObject.SetActive(networkSession == null || networkSession.connected);
        buttonText.text = networkSession == null ? "Connect" : "Disconnect";

        statusText.text = networkSession == null ? "Disconnected" : $"{networkSession.status}";
        
        // room info
        {
            if (RoomManager.current is NetworkRoom room) {
                roomInfoPanel.SetActive(true);
                roomStatusText.text = $"Status: {room.status}";;
                playerCountText.text = $"Expected players: {room.playerCount}";
                
                spectatorCountText.text = $"Expected spectators: {room.spectatorCount}";
                matchAcceptPanel.SetActive(room.status == RoomStatus.WAIT_ACCEPT);
                matchAcceptText.text = room.matchAccepted ? "Match accepted" : "Match is ready";
                acceptMatchButton.interactable = declineMatchButton.interactable = !room.matchAccepted;
                
            } else {
                roomInfoPanel.SetActive(false);
            }
        }
        
    }

    private void OnButtonPress() {
        if (networkSession == null) {
            Connect();
        } else {
            Disconnect();
        }
    }
    
    private void Connect() {
        var host = hostInput.text;
        var port = int.Parse(portInput.text);

        var baseConfiguration = Instantiate(clientConfiguration);
        baseConfiguration.host = host;
        baseConfiguration.port = port;
        
        // generate random string 15 chr
        baseConfiguration.userId = "debug-" + Random.Range(0, 1000000).ToString("D6");
        print($"Using userId: {baseConfiguration.userId}");
        
        NetworkManager.inst.BeginSession(baseConfiguration, roomConfiguration);
    }

    private void AcceptMatch(bool accepted) {
        (RoomManager.current as NetworkRoom)?.NotifyMatchAccept(accepted);
    }

    private void Disconnect() {
        if (networkSession == null) return;
        networkSession.Disconnect(ClientDisconnectionReason.BACK_TO_LOBBY, "debug disconnect");
    }

    private async void TestStun() {
        print("testing stun");
        var ret = await StunClient.GetPublicEndpointAsync(6023);
        print($"STUN result: {ret}");
    }
}
}
