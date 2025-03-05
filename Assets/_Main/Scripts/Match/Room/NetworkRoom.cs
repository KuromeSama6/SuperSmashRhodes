using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Config.Global;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.GGPOWrapper;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Match.Player;
using SuperSmashRhodes.Network.Rollbit;
using SuperSmashRhodes.Network.Rollbit.P2P;
using SuperSmashRhodes.UI.Global.LoadingScreen;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace SuperSmashRhodes.Network.RoomManagement {
public class NetworkRoom : Room, IPacketHandler {
    public bool matchAccepted { get; private set; }
    public NetworkSession session { get; private set; }
    public GGPOConnector ggpo { get; private set; }
    public bool fighting { get; private set; }
    public int roundsWon => GetWinCount(localPlayer.playerId);
    public IRandomNumberProvider randomNumberProvider { get; private set; } = new DefaultRandomNumberProvider();
    public NetworkInputBufferWrapper remoteBuffer { get; } = new();
    
    private readonly Dictionary<string, Player> idMap = new();
    
    public override bool allConfirmed => status == RoomStatus.NEGOTIATING;
    public NetworkLocalPlayer localPlayer => idMap[session.config.userId] as NetworkLocalPlayer;
    public NetworkRemotePlayer remotePlayer => players[localPlayer.playerId == 0 ? 1 : 0] as NetworkRemotePlayer;
    public InputChord localThisFrameInputs {
        get {
            if (!fighting) return null;
            if (localPlayer == null || !localPlayer.playerCharacter) return null;
            var ret = localPlayer.playerCharacter.inputProvider.inputBuffer.thisFrame;
            return ret;
        }
    }
    
    public NetworkRoom(RoomConfiguration configuration, NetworkSession session) : base(configuration) {
        this.session = session;
        session.AddPacketHandler(this);
        
        this.session.onDisconnected.AddListener(OnDisconnected);
    }
    
    public void NotifyMatchAccept(bool accepted) {
        if (status != RoomStatus.WAIT_ACCEPT || matchAccepted) return;

        session.SendPacket<PacketPlayOutGenericResponse>(new PacketPlayInAcceptMatch(session, accepted))
            .ContinueWith(p => {
                matchAccepted = p.Result.header.statusCode == 0;
            });
    }

    public void BeginMatchLocal() {
        //TODO stage and bgm selections
        stageData = StageDatabase.inst.stages[0];
        bgmData = StageDatabase.inst.bgms[0];
        
        StartCoroutine(LoadMatchRoutine());
    }
    
    private void OnDisconnected() {
        Debug.LogWarning("Disconnected from server, destroying room");
        Destroy();
        
        MainThreadDispatcher.RunOnMain(() => {
            ggpo?.Dispose();
        });
    }
    
    private Task<PacketPlayOutGenericResponse> ReportRoundStatus(bool fighting) {
        return session.SendPacket<PacketPlayOutGenericResponse>(new PacketPlayInRoundStatus(session, fighting, this));
    }
    
    // Packet handlers
    [PacketHandler]
    public void OnRoomStatusUpdate(PacketPlayOutRoomStatus packet) {
        Debug.Log($"Room status update: {packet.status}, fighting: {packet.fighting} players: {String.Join(", ", packet.userIds)}");
        
        if (status != packet.status) {
            status = packet.status;

            if (status == RoomStatus.CHARACTER_SELECT) {
                // create players
                foreach (var userId in packet.userIds) {
                    var playerId = freePlayerId;
                    Debug.Log(userId);

                    Player player;
                    if (userId == session.config.userId) {
                        player = new NetworkLocalPlayer(this, playerId, userId);
                
                    } else {
                        player = new NetworkRemotePlayer(this, playerId, userId);
                    }
                    
                    idMap[userId] = player;
                    AddPlayer(player);
                }

                Debug.Log("Starting character select routine");
                StartCoroutine(ShowCharacterSelectRoutine());
            }

            if (status == RoomStatus.NEGOTIATING && session.config.debugSkipCharacterSelect) {
                MainThreadDispatcher.RunOnMain(BeginMatchLocal);
            }
        }
        
        if (fighting != packet.fighting) {
            fighting = packet.fighting;

            // inputManager.active = fighting; //TODO put back
            // if (fighting) {
            //     inputManager.Reset();
            //     GameStateManager.inst.ResetFrame();
            //     // MainThreadDispatcher.RunOnMain(() => Time.timeScale = .1f);
            // }
        }
    }

    [PacketHandler]
    public void OnCharacterSelectUpdate(PacketPlayOutCharacterSelect packet) {
        // Debug.Log($"character select update: {packet.userId}, {packet.index}, {packet.confirmed}");
        var player = idMap[packet.userId];

        lock (player) {
            MainThreadDispatcher.RunOnMain(() => {
                player.character = CharacterDescriptor.FromIndex(packet.index);
                player.characterConfirmed = packet.confirmed;
            });   
        }
    }

    [PacketHandler]
    public void OnBeginP2P(PacketPlayOutBeginP2P packet) {
        MainThreadDispatcher.RunOnMain(() => {
            ggpo.BeginP2P(packet); 
        });
    }

    [PacketHandler]
    public void OnReceivePreRandom(PacketPlayOutPreRandom packet) {
        Debug.Log($"Received prerandom, {packet}");
        randomNumberProvider = new PredeterminedRandom(packet.randoms);
    }
     
    //region routines

    private IEnumerator ShowCharacterSelectRoutine() {
        foreach (var (id, player) in players) {
            player.character = CharacterDescriptor.FromIndex(id);
        }

        if (session.config.debugSkipCharacterSelect) {
            foreach (var (id, player) in players) {
                if (player is NetworkLocalPlayer networkLocalPlayer) {
                    networkLocalPlayer.SetConfirmed(true);
                }
            }
            yield break;
        }
        
        var loadingScreen = LoadingScreen.inst;
        loadingScreen.visible = true;
        loadingScreen.SetLoadingChecklist(
            "等待玩家",
            "角色选择页面"
        );

        loadingScreen.showCover = true;
        yield return new WaitForSeconds(1f);
        
        loadingScreen.UpdateLoadingStatus(LoadingStatus.GOOD);
        
        // load scene
        var handle = SceneManager.LoadSceneAsync("CharacterSelect");
        if (handle == null) {
            Debug.LogError("Failed to load scene: CharacterSelect");
            loadingScreen.UpdateLoadingStatus(LoadingStatus.BAD);
            session.Disconnect(ClientDisconnectionReason.CLIENT_ERROR, "Failed to load scene: CharacterSelect");
            yield break;
        }
        
        handle.allowSceneActivation = false;
        
        while (!handle.isDone) {
            if (handle.progress >= 0.9f) {
                loadingScreen.showCover = true;
                
                yield return new WaitForSeconds(1);
                handle.allowSceneActivation = true;
                loadingScreen.UpdateLoadingStatus(LoadingStatus.GOOD);
                break;
            }
            
            yield return null;
        }

        handle.allowSceneActivation = true;
        loadingScreen.visible = false;
    }

    private IEnumerator LoadMatchRoutine() {
        var loadingScreen = LoadingScreen.inst;
        loadingScreen.visible = true;
        loadingScreen.showCover = false;
        loadingScreen.SetLoadingChecklist(
            "网络状态",
            "SAELN服务",
            "IP地址",
            "P2P连接",
            "同步数据"
        );
        
        // check network status
        using (var req = UnityWebRequest.Get("https://1.1.1.1")) {
            req.timeout = 10;
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success) {
                Debug.LogError("Network test failed.");
                loadingScreen.UpdateLoadingStatus(LoadingStatus.BAD);
                session.Disconnect(ClientDisconnectionReason.CLIENT_ERROR, "Network test failed.");
                yield break;
            }
        }
        
        loadingScreen.UpdateLoadingStatus(LoadingStatus.GOOD);
        loadingScreen.UpdateLoadingStatus(LoadingStatus.NA); //TODO: SAELN service check
        
        //P2P connection
        // find free port
        ggpo = new(this);
        var bindSuccess = ggpo.Bind();
        
        if (!bindSuccess) {
            loadingScreen.UpdateLoadingStatus(LoadingStatus.BAD);
            yield break;
        }
        
        // send packet
        {
            var task = session.SendPacket(new PacketPlayInNegotiate(session, ggpo.port));
            yield return new WaitUntil(() => task.IsCompleted);
            if (!task.IsCompletedSuccessfully || task.Result == null) {
                Debug.LogError("Failed to send P2P negotiation packet. Task error.");
                loadingScreen.UpdateLoadingStatus(LoadingStatus.BAD);
                session.Disconnect(ClientDisconnectionReason.CLIENT_ERROR, "Failed to send P2P negotiation packet (task error).");
                yield break;
            }
            
            loadingScreen.UpdateLoadingStatus(LoadingStatus.GOOD);
        }
        
        // negotiate
        // debug loop
        while (true) {
            yield return null;
        }
        
        while (ggpo.status == GGPOConnectionStatus.CONNECTING) {
            yield return null;
        }

        if (ggpo.connected) {
            loadingScreen.UpdateLoadingStatus(LoadingStatus.GOOD);
            
        } else {
            Debug.LogError("Negotiation failed and no fallback available.");
            session.Disconnect(ClientDisconnectionReason.CLIENT_ERROR, "negotiation failed.");
            loadingScreen.UpdateLoadingStatus(LoadingStatus.BAD);
            yield break;
        }
        
        while (ggpo.status == GGPOConnectionStatus.SYNCHRONIZING) {
            yield return null;
        }
        
        if (ggpo.status == GGPOConnectionStatus.ESTABLISHED) {
            loadingScreen.UpdateLoadingStatus(LoadingStatus.GOOD);
            
        } else {
            Debug.LogError("Initial synchronization failure.");
            session.Disconnect(ClientDisconnectionReason.CLIENT_ERROR, "initial synchronization failure");
            loadingScreen.UpdateLoadingStatus(LoadingStatus.BAD);
            yield break;
        }
        
        yield return LoadMatchSceneRoutine();
        
        // report status
        GameManager.inst.random = randomNumberProvider;
        StartMatch();
    }

    protected override IEnumerator RoundStartRoutine() {
        // wait for room status
        ReportRoundStatus(true);
        while (!fighting) {
            yield return null;
        }
        
        yield return base.RoundStartRoutine();
    }

    //endregion
}
}
