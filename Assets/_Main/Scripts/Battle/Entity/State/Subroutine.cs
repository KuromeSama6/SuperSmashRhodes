using SuperSmashRhodes.Battle.Serialization;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State {
public delegate void EntityStateSubroutine(SubroutineContext ctx);
public delegate EntityStateSubroutine DeferedNextSubroutine();

public class SubroutineContext : IHandleSerializable {
    public int interruptFrames { get; private set; }
    public string nextState { get; private set; }
    public EntityStateSubroutine nextSubroutine { get; private set; }
    public DeferedNextSubroutine deferedNextSubroutine { get; private set; }
    public SubroutineFlags flags { get; set; }
    
    public EntityStateSubroutine subroutine { get; private set; }
    public SubroutineContext(EntityStateSubroutine subroutine) {
        if (subroutine == null) throw new System.ArgumentNullException(nameof(subroutine));
        this.subroutine = subroutine;
    }
    
    private SubroutineContext() {}
    
    public bool isDefered => deferedNextSubroutine != null;
    
    public void Next(int interrupt, string nextState = null) {
        interruptFrames = interrupt;
        this.nextState = nextState;
    }

    public void Next(int interrupt, EntityStateSubroutine nextSubroutine) {
        interruptFrames = interrupt;
        this.nextSubroutine = nextSubroutine;
    }

    public void Exit(int interrupt = 0) {
        interruptFrames = interrupt;
        nextSubroutine = null;
        nextState = null;
    }
    
    public void Repeat(int interrupt = 1) {
        Next(interrupt, subroutine);
    }
    
    public void Defer(int interrupt, DeferedNextSubroutine deferedNextSubroutine) {
        interruptFrames = interrupt;
        this.deferedNextSubroutine = deferedNextSubroutine;
    }

    public IHandle GetHandle() {
        return new Handle(this);
    }

    private struct Handle : IHandle {
        public readonly int interruptFrames;
        public readonly string nextState;
        public readonly EntityStateSubroutine nextSubroutine;
        public readonly DeferedNextSubroutine deferedNextSubroutine;
        public readonly SubroutineFlags flags;
        public readonly EntityStateSubroutine subroutine;

        public Handle(SubroutineContext ctx) {
            interruptFrames = ctx.interruptFrames;
            nextState = ctx.nextState;
            nextSubroutine = ctx.nextSubroutine;
            deferedNextSubroutine = ctx.deferedNextSubroutine;
            flags = ctx.flags;
            subroutine = ctx.subroutine;
        }
        
        public object Resolve() {
            return new SubroutineContext {
                interruptFrames = interruptFrames,
                nextState = nextState,
                nextSubroutine = nextSubroutine,
                deferedNextSubroutine = deferedNextSubroutine,
                flags = flags,
                subroutine = subroutine
            };
        }
    }
}

}
