using System;
using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.Battle.Enums;
using UnityEngine;

namespace SuperSmashRhodes.FScript.Input {
/// <summary>
/// An InputBuffer is a queue of inputs that stores abstractions of player inputs.
/// This is the only source of input data that FScript will read from.
/// </summary>
public class InputBuffer {
    public int maxSize { get; set; }
    public List<InputChord> buffer { get; } = new();
    public InputChord thisFrame => buffer[0];
    
    public InputBuffer(int maxSize) {
        this.maxSize = maxSize;
        for (int i = 0; i < maxSize; i++) buffer.Add(new InputChord());
    }
    
    public InputBuffer(int maxSize, InputChord[] buffer) : this(maxSize) {
        this.buffer = buffer.ToList();
    }

    public void PushAndTick(params InputFrame[] inputs) {
        InputChord chord = new(inputs);
        buffer.Insert(0, chord);
        
        if (buffer.Count > maxSize) buffer.RemoveAt(buffer.Count - 1);
    }

    public InputBuffer Slice(int frames) {
        var ret = buffer.GetRange(0, frames);
        return new InputBuffer(maxSize, ret.ToArray());
    }
    
    public static InputType TranslateRawDirectionInput(InputType input, EntityFacing facing) {
        if (input == InputType.RAW_MOVE_LEFT) {
            if (facing == EntityFacing.LEFT) return InputType.FORWARD;
            return InputType.BACKWARD;

        }
        if (input == InputType.RAW_MOVE_RIGHT) {
            if (facing == EntityFacing.LEFT) return InputType.BACKWARD;
            return InputType.FORWARD;

        }
        throw new ArgumentException("Input must be RAW_MOVE_LEFT or RAW_MOVE_RIGHT");
    }
}

public enum InputFrameType {
    PRESSED,
    HELD,
    RELEASED
}

public struct InputFrame : IEquatable<InputFrame> {
    public InputType type { get; }
    public InputFrameType frameType { get; }
    
    public InputFrame(InputType type, InputFrameType frameType) {
        this.type = type;
        this.frameType = frameType;
    }
    
    public bool Equals(InputFrame other) {
        return type == other.type && frameType == other.frameType;
    }
    public override int GetHashCode() {
        return HashCode.Combine((int)type, (int)frameType);
    }
}

public class InputChord {
    public InputFrame[] inputs;

    public InputChord(params InputFrame[] inputs) {
        this.inputs = inputs;
    }

    public bool HasInput(InputFrame type) {
        return inputs.Contains(type);
    }

    public override string ToString() {
        return $"InputChord({string.Join(", ", inputs)})";
    }
}
}
