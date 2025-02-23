using System;
using System.Security.Cryptography;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Match;
using SuperSmashRhodes.Network.Rollbit;
using SuperSmashRhodes.Network.Room;
using UnityEngine;

namespace SuperSmashRhodes.Network {
public class NetworkManager : AutoInitSingletonBehaviour<NetworkManager> {
    public NetworkSession currentSession { get; private set; }
    public bool isConnected => currentSession != null && currentSession.connected;
    
    public void BeginSession(RollbitClientConfiguration configuration, RoomConfiguration roomConfiguration) {
        if (isConnected) return; 
        currentSession = new NetworkSession(configuration);
        currentSession.onEstablished.AddListener(() => MainThreadDispatcher.RunOnMain(() => CreateRoom(roomConfiguration)));
        currentSession.onDisconnected.AddListener(() => {
            currentSession.Dispose();
            currentSession = null;
        });
    }

    private void CreateRoom(RoomConfiguration configuration) {
        var room = new NetworkRoom(configuration, currentSession);
        RoomManager.inst.CreateRoom(room);
    }
    
    private void OnDestroy() {
        currentSession?.Dispose();
    }

}
}
