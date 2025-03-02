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

    [Title("Debug")]
    public bool debugSkipCharacterSelect;
}
}
