using System;

namespace SuperSmashRhodes.Battle.Game {
[Flags]
public enum CharacterStateFlag {
    NONE = 0,
    
    PAUSE_PHYSICS = 1 << 0,
    PAUSE_ANIMATIONS = 1 << 1,
    DISABLE_BURST = 1 << 2,
    PAUSE_STATE = 1 << 3,
    NO_CAMERA_WEIGHT = 1 << 4,
    CAMERA_FOLLOW_BONE = 1 << 5,
    
    TIME_THROW = PAUSE_PHYSICS | DISABLE_BURST | PAUSE_STATE,
    TIME_SUPER = PAUSE_PHYSICS | DISABLE_BURST | PAUSE_STATE,
    TIME_SUPER_CINEMATIC = TIME_SUPER,
}
}
