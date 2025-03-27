namespace SuperSmashRhodes.Input {
public enum InputType {
    NONE = 0,
    FORWARD = 6,
    BACKWARD = 4,
    UP = 8,
    DOWN = 2,
    P = 10,
    S = 12, 
    HS = 13, 
    D = 14,
    DASH = 15,
    FORCE_RESET = 16,
    
    RAW_MOVE_RIGHT = 98,
    RAW_MOVE_LEFT = 99,
    
    // Control sequences
    /// <summary>
    /// Simulates a clearing of the current input buffer. During networked games, in order to ensure determinism, the input buffer is oftentimes not modifiable (appart from appending to it). To simulate a clearing of the buffer, this input type can be appended into an input buffer, which causes any reads or scans of the buffer to immediately end upon reaching this escape sequence, effectively clearing the buffer.
    /// </summary>
    ESC_CLEAR_BUFFER = 1000,
    /// <summary>
    /// When encountered during InputBuffer.TimeSlice, instructs the time slice to include the next frame in the buffer, even it is beyond the specified length of the time slice. This allows inputs pressed during a freeze/hitstop to still be read and processed immediately after the freeze/hitstop ends.
    /// </summary>
    ESC_FREEZE_TIMESLICE_CONTINUE = 1001,
}
}
