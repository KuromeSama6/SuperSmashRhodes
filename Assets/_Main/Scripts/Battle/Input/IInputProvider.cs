namespace SuperSmashRhodes.Input {
public interface IInputProvider {
    InputBuffer inputBuffer { get; }
    
}

public class NOPInputProvider : IInputProvider {
    public InputBuffer inputBuffer { get; } = new InputBuffer(10);
}
}
