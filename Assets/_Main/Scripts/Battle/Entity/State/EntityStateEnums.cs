using System;
using System.Numerics;

namespace SuperSmashRhodes.Battle.State {
[Flags]
public enum EntityStateType : ulong {
    ALL = ulong.MaxValue,
    CHR_NEUTRAL = 1 << 0,
    CHR_MOVEMENT_LOOP = 1 << 1,
    
    CHR_ATK_SPECIAL = 1 << 4,
    CHR_ATK_SUPER = 1 << 5,
    CHR_MOVEMENT_SINGLE = 1 << 6,
    CHR_ATK_SYSTEMSPECIAL = 1 << 7,
    
    CHR_ATK_5P = 1 << 8,
    CHR_ATK_5K = 1 << 9,
    CHR_ATK_5S = 1 << 10,
    CHR_ATK_5CS = 1 << 11,
    CHR_ATK_5H = 1 << 12,
    CHR_ATK_5D = 1 << 13,
    
    CHR_ATK_6P = 1 << 14,
    CHR_ATK_6H = 1 << 15,
    
    CHR_ATK_2P = 1 << 16,
    CHR_ATK_2K = 1 << 17,
    CHR_ATK_2S = 1 << 18,
    CHR_ATK_2H = 1 << 19,
    CHR_ATK_2D = 1 << 20,
    
    CHR_ATK_8P = 1 << 21,
    CHR_ATK_8K = 1 << 22,
    CHR_ATK_8S = 1 << 23,
    CHR_ATK_8H = 1 << 24,
    CHR_ATK_8D = 1 << 25,
    
    CHR_BLOCKSTUN = 1 << 26,
    CHR_HITSTUN = 1 << 27,
    
    CHR_ATK_NORMAL = CHR_ATK_5P | CHR_ATK_5K | CHR_ATK_5S | CHR_ATK_5CS | CHR_ATK_5H | CHR_ATK_5D | CHR_ATK_6P | CHR_ATK_6H | CHR_ATK_2P | CHR_ATK_2K | CHR_ATK_2S | CHR_ATK_2H | CHR_ATK_2D,
    CHR_ATK_AIR_NORMAL = CHR_ATK_8P | CHR_ATK_8K | CHR_ATK_8S | CHR_ATK_8H | CHR_ATK_8D,
    CHR_ATK_SPECIAL_SUPER = CHR_ATK_SPECIAL | CHR_ATK_SUPER,
    CHR_STUN = CHR_BLOCKSTUN | CHR_HITSTUN,
    CHR_ATK_ALL = CHR_ATK_NORMAL | CHR_ATK_SPECIAL | CHR_ATK_SUPER | CHR_ATK_SYSTEMSPECIAL | CHR_ATK_AIR_NORMAL,
}
}
