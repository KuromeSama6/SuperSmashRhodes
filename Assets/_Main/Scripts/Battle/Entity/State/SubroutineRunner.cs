using System;
using SuperSmashRhodes.Battle.Serialization;

namespace SuperSmashRhodes.Battle.State {
public class SubroutineRunner : IStateSerializable {
    public EntityStateSubroutine subroutine { get; private set; }
    public SubroutineContext context { get; private set; }
    public bool called { get; private set; }
    
    public SubroutineRunner(EntityStateSubroutine subroutine) {
        this.subroutine = subroutine;
        context = new SubroutineContext(subroutine);
    }

    public void Call() {
        if (called)
            throw new InvalidOperationException("Subroutine has already been called");
        
        subroutine.Invoke(context);
        called = true;
    }

    public void Switch(EntityStateSubroutine subroutine) {
        this.subroutine = subroutine;
        context = new SubroutineContext(subroutine);
        called = false;
    }
    
    public bool NextState() {
        called = false;
        if (context.isDefered) {
            subroutine = context.deferedNextSubroutine.Invoke();
            if (subroutine == null) return false;
            context = new SubroutineContext(subroutine);
            return true;
        }

        if (context.nextState != null) {
            return true;
        }
        
        if (context.nextSubroutine != null) {
            subroutine = context.nextSubroutine;
            context = new SubroutineContext(subroutine);
            return true;
        }
        
        return false;
    }

    public void Serialize(StateSerializer serializer) {
        serializer.PutReference("subroutine", subroutine);
        serializer.Put("context", context);
        serializer.Put("called", called);
    }
    public void Deserialize(StateSerializer serializer) {
        subroutine = serializer.GetReference<EntityStateSubroutine>("subroutine");
        context = serializer.GetHandle<SubroutineContext>("context");
        called = serializer.Get<bool>("called");
    }
}
}
