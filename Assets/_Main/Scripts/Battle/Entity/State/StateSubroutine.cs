using System;
using System.Collections;
using SuperSmashRhodes.Battle.Serialization;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State {
public class StateSubroutine : IHandleSerializable {
    public RoutineSource source { get; private set; }
    public IEnumerator enumerator { get; private set; }
    public int parentFrame { get; private set; }
    public SubroutineFlags flags { get; private set; }
    public int timesTicked { get; set; }

    public bool hydrated => enumerator != null;

    public StateSubroutine(RoutineSource source, int parentFrame, SubroutineFlags flags = SubroutineFlags.NONE) {
        this.source = source;
        this.parentFrame = parentFrame;
        this.flags = flags;
    }

    public void Hydrate() {
        enumerator = source.Invoke();
        timesTicked = 0;
    }

    public IHandle GetHandle() {
        return new StateSubroutineHandle(this);
    }

    public override string ToString() {
        return $"StateSubroutine(source={source}, parentFrame={parentFrame}, flags={flags}, timesTicked={timesTicked})";
    }

}

public struct StateSubroutineHandle : IHandle {
    private RoutineSource source;
    public int parentFrame;
    public SubroutineFlags flags;
    public int timesTicked;
    
    public StateSubroutineHandle(StateSubroutine routine) {
        source = routine.source;
        parentFrame = routine.parentFrame;
        flags = routine.flags;
        timesTicked = routine.timesTicked;
    }
    
    public object Resolve() {
        var ret = new StateSubroutine(source, parentFrame, flags);
        ret.Hydrate();
        
        // restore enumerator state
        // Debug.Log($"reconstruct enumerator, time ticked = {timesTicked}");
        for (int i = 0; i < timesTicked; i++) {
            ret.enumerator.MoveNext();
            ++ret.timesTicked;
        }
        
        return ret;
    }

    public override string ToString() {
        return $"StateSubroutineHandle(source={source}, parentFrame={parentFrame}, flags={flags}, timesTicked={timesTicked})";
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
