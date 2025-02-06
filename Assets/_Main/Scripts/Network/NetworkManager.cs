using System;
using System.Security.Cryptography;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Network.Rollbit;
using SuperSmashRhodes.Network.Room;
using UnityEngine;

namespace SuperSmashRhodes.Network {
public class NetworkManager : AutoInitSingletonBehaviour<NetworkManager> {
    public NetworkSession currentSession { get; private set; }
    public bool isConnected => currentSession != null && currentSession.connected;
    
    public RoomManager room { get; private set; }

    public void BeginSession(RollbitClientConfiguration configuration) {
        if (isConnected) return; 
        currentSession = new NetworkSession(configuration);
        currentSession.onEstablished.AddListener(() => MainThreadDispatcher.RunOnMain(CreateRoomManger));
        currentSession.onDisconnected.AddListener(() => {
            currentSession.Dispose();
            currentSession = null;
            MainThreadDispatcher.RunOnMain(() => Destroy(room.gameObject));
        });
    }

    private void CreateRoomManger() {
        var go = new GameObject();
        go.transform.parent = transform;
        go.name = "RoomManager";
        room = go.AddComponent<RoomManager>();
        room.Init(currentSession);
    }
    
    private void OnDestroy() {
        currentSession?.Dispose();
    }

}
}
