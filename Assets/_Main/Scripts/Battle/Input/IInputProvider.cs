namespace SuperSmashRhodes.Input {
public interface IInputProvider {
    InputBuffer inputBuffer { get; }

    void SetBuffer(InputBuffer newBuffer);
}

public class NOPInputProvider : IInputProvider {
    public InputBuffer inputBuffer { get; } = new InputBuffer(10);
    public void SetBuffer(InputBuffer newBuffer) {
        
    }
}
}
