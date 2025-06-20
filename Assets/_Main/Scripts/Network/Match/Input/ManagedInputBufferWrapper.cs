﻿using SuperSmashRhodes.Input;

namespace SuperSmashRhodes.Network {
public class ManagedInputBufferWrapper : IInputProvider {
    public InputBuffer inputBuffer { get; private set; } = new(120);
    public void SetBuffer(InputBuffer newBuffer) {
        inputBuffer = newBuffer;
    }
}
}
