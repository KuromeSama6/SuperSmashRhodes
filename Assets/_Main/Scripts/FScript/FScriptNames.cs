using System;
using SuperSmashRhodes.FScript.Enums;

namespace SuperSmashRhodes._Main.Scripts.FScript {

public static class FScriptRegister {
    public static readonly string DAMAGE = "dmg";
    public static readonly string HITSTUN = "hit";
    public static readonly string BLOCKSTUN = "block";
    [FScriptTokenInit(GuardType.ALL)]
    public static readonly string GUARD_TYPE = "guard";
    [FScriptTokenInit(CounterHitType.SMALL)]
    public static readonly string COUNTER_TYPE = "ch";
    [FScriptTokenInit(HitState.NONE)]
    public static readonly string HIT_STATE = "hitstate";

    public static readonly string GENERAL_A = "rax";
    public static readonly string GENERAL_B = "rbx";
    public static readonly string GENERAL_C = "rcx";
    public static readonly string GENERAL_D = "rdx";
    public static readonly string SUBROUTINE_RETURN = "rrp";
    public static readonly string CURRENT_INSTRUCTION_PTR = "rci";
    public static readonly string LAST_INSTRUCTION_PTR = "rpi";
    
    public static readonly string ENT_HP = "ent_hp";
    public static readonly string ENT_MAX_HP = "ent_maxhp";
    public static readonly string ENT_DEFENSE = "ent_defense";
    public static readonly string ENT_WALK_SPEED = "ent_walk_spd";
    public static readonly string ENT_BACKWALK_SPEED = "ent_backwalk_spd";
    public static readonly string ENT_PREJUMP_F = "ent_prejump_f";
    public static readonly string ENT_DASH_SPEED = "ent_dash_spd";
    
    public static readonly string CHR_PREJUMP_LEN = "prejumplen";
    public static readonly string CHR_JUMP_LEN = "jumplen";
    public static readonly string CHR_JUMP_VEL = "jumpvel";
}

public static class FScriptConstant {
    [FScriptTokenInit(true)]
    public static readonly string PHY_GROUNDED = "p_grounded";
    [FScriptTokenInit(true)]
    public static readonly string GROUNDED = "l_grounded";
}

[AttributeUsage(AttributeTargets.Field)]
public class FScriptTokenInit : Attribute {
    public object value { get; }
    public FScriptTokenInit(object value) {
        this.value = value;
    }
}
}
