using System;
using System.Collections;

namespace SuperSmashRhodes.Battle.State {
public class SubroutineWrapper {
    public IEnumerator enumerator { get; private set; }
    public int parentFrame { get; private set; }
    public SubroutineFlags flags { get; private set; }
    
    public SubroutineWrapper(IEnumerator enumerator, int parentFrame, SubroutineFlags flags = SubroutineFlags.NONE) {
        this.enumerator = enumerator;
        this.parentFrame = parentFrame;
        this.flags = flags;
    }
}

[Flags]
public enum SubroutineFlags {
    NONE = 0,
    PAUSE_ANIMATION = 1 << 0,
}
}
