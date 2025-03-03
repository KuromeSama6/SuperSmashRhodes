namespace SuperSmashRhodes.Battle.State {
public delegate void EntityStateSubroutine(ref SubroutineReturnOptions options);

public struct SubroutineReturnOptions {
    public int interruptFrames;
    public string nextState;
}

}
