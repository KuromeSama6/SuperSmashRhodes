using System;

namespace SuperSmashRhodes.Battle.Game {
public class UnmanagedTimeSlot {
    public int frames { get; set; }
    public bool valid => frames > 0;
    public UnmanagedTimeSlotFlags flags { get; private set; }

    public UnmanagedTimeSlot(int frames, UnmanagedTimeSlotFlags flags) {
        this.frames = frames;
        this.flags = flags;
    }
    
}

public struct CharacterUnmanagedTimeData {
    public int frames;
    public UnmanagedTimeSlotFlags flags;
}

[Flags]
public enum UnmanagedTimeSlotFlags {
    NONE = 0,
    
    PAUSE_PHYSICS = 1 << 0,
    PAUSE_ANIMATIONS = 1 << 1,
    DISABLE_BURST = 1 << 2,
    PAUSE_STATE = 1 << 3,
    
    TIME_THROW = PAUSE_PHYSICS | DISABLE_BURST | PAUSE_STATE,
}
}
