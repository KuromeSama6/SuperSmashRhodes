using System;

namespace SuperSmashRhodes.Network.Rollbit.P2P {
public enum GGPOConnectionStatus { 
    STANDBY,
    CONNECTING,
    CONNECTED_WAIT,
    SYNCHRONIZING,
    SYNCHRONIZED,
    ESTABLISHED,
    PAUSED,
    DISCONNECTED,
    
    [Obsolete]
    SKIPPED
}
}
