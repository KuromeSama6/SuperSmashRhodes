using SuperSmashRhodes.Network.Rollbit;
using UnityEngine.Events;

namespace SuperSmashRhodes.Network.Room {
public abstract class Room {
    public RoomConfiguration config { get; private set; }
    public int playerCount { get; protected set; } = 0;
    public int spectatorCount { get; protected set; } = 0;
    
    public UnityEvent onDestroy { get; } = new();
    public UnityEvent<RoomStatus> onStatusChange { get; } = new();


    public RoomStatus status {
        get => _status;
        protected set {
            if (value != status) {
                onStatusChange.Invoke(value);
            }
            _status = value;
        }
    }
    private RoomStatus _status = RoomStatus.STANDBY;
    
    public Room(RoomConfiguration config) {
        this.config = config;
    }

    public void Destroy() {
        onDestroy.Invoke();
        onDestroy.RemoveAllListeners();
    }
}

public enum RoomStatus : ushort {
    STANDBY = 0,
    WAIT_ACCEPT = 1,
    CHARACTER_SELECT = 2,
    NEGOTIATING = 3,
    CLIENT_LOADING = 4,
    PLAYING = 5,
    FINISHED = 6
}
}
