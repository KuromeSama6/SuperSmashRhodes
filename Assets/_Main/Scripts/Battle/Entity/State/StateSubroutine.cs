using System;
using System.Collections;
using SuperSmashRhodes.Battle.Serialization;

namespace SuperSmashRhodes.Battle.State {
public class StateSubroutine : IReflectionSerializable {
    public RoutineSource source { get; private set; }
    public IEnumerator enumerator { get; private set; }
    public int parentFrame { get; private set; }
    public SubroutineFlags flags { get; private set; }
    public ReflectionSerializer reflectionSerializer { get; }
    public int timesTicked { get; set; }

    public bool hydrated => enumerator != null;

    public StateSubroutine(RoutineSource source, int parentFrame, SubroutineFlags flags = SubroutineFlags.NONE) {
        this.source = source;
        this.parentFrame = parentFrame;
        this.flags = flags;

        reflectionSerializer = new(this);
    }

    public void Hydrate() {
        enumerator = source.Invoke();
        timesTicked = 0;
    }

}

[SerializationOptions(SerializationOption.DIRECT_REFERENCE)] 
public delegate IEnumerator RoutineSource();

[Flags]
public enum SubroutineFlags {
    NONE = 0,
    PAUSE_ANIMATION = 1 << 0,
}
}
