using System.Net.Sockets;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.Network {
[CreateAssetMenu(menuName = "SSR/Rollbit/Configuration", order = 0)]
public class RollbitClientConfiguration : ScriptableObject {
    public string host;
    public int port;
    public string userId;
    public string aesKey;
    
    [Title("Network")]
    public int p2PNegotiationAttempts = 10;
    public NetcodeMode netcodeMode = NetcodeMode.ROLLBACK;
    public int maxRollbackFrames = 7;
    
    [Title("Debug")]
    public bool debugSkipCharacterSelect;
    public bool debugP2PLatency;
    public Vector2Int debugP2PLatencyRange;
}
}
