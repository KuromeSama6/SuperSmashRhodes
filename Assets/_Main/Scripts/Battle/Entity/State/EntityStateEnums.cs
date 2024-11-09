using System;

namespace SuperSmashRhodes.Battle.State {
[Flags]
public enum EntityStateType {
    ALL = -1,
    
    CHR_NEUTRAL = 1 << 0,
    CHR_MOVEMENT_LOOP = 1 << 1,
    CHR_MOVE_NORMAL = 1 << 2,
    CHR_MOVE_CMD_NORMAL = 1 << 3,
    CHR_MOVE_SPECIAL = 1 << 4,
    CHR_MOVE_SUPER = 1 << 5,
    CHR_MOVEMENT_SINGLE = 1 << 6,
}
}
