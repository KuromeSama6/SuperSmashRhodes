using System;
using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.Battle.Enums;
using UnityEngine;

namespace SuperSmashRhodes.Input {
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
        foreach (var c in buffer) {
            if (c.consumed) c.Consume();
        }
    }
    
    public void PushToCurrentFrame(params InputFrame[] inputs) {
        buffer[0] = new(buffer[0].inputs.Concat(inputs).ToArray());
    }

    public InputBuffer TimeSlice(int frames) {
        var ret = buffer.GetRange(0, frames);
        return new InputBuffer(maxSize, ret.ToArray());
    }

    public bool ScanForInput(EntitySide side, params InputFrame[] seq) {
        List<InputFrame[]> args = new();
        foreach (var frame in seq) {
            args.Add(new[]{ frame });
        }
        return ScanForInput(side, args.ToArray());
    }
    
    public bool ScanForInput(EntitySide side, params InputFrame[][] seq) {
        var req = seq.ToList();
        if (req.Count == 0)
            throw new ArgumentException("Input sequence must have at least one element");

        bool result = false;
        List<InputChord> toConsume = new();
        
        for (int i = buffer.Count - 1; i >= 0; i--) {
            if (buffer[i].HasInput(side, InputType.ESC_CLEAR_BUFFER, InputFrameType.PRESSED)) {
                break;
            }
            
            if (req[0].ToList().TrueForAll(c => buffer[i].HasInput(side, c))) {
                // Debug.Log(string.Join(", ", buffer));
                req.RemoveAt(0);
                toConsume.Add(buffer[i]);
                if (req.Count == 0) {
                    result = true;
                    break;
                }
            }
        }

        if (result) {
            foreach (var c in toConsume) {
                c.consumed = true;
            }
        }
        return result;
    }

    public bool HasInputUnordered(EntitySide side, params InputFrame[] seq) {
        return seq.ToList().TrueForAll(c => buffer.Any(d => d.HasInput(side, c)));
    }

    public void SimulatedClear() {
        PushAndTick(new InputFrame(InputType.ESC_CLEAR_BUFFER, InputFrameType.PRESSED));
    }
    
    public static InputType TranslateToRawDirection(InputType input, EntitySide side) {
        if (input == InputType.FORWARD) {
            if (side == EntitySide.RIGHT) return InputType.RAW_MOVE_RIGHT;
            return InputType.RAW_MOVE_LEFT;

        }
        if (input == InputType.BACKWARD) {
            if (side == EntitySide.RIGHT) return InputType.RAW_MOVE_LEFT;
            return InputType.RAW_MOVE_RIGHT;

        }
        
        return input;
    }
}

public enum InputFrameType {
    PRESSED,
    HELD,
    RELEASED
}

public struct InputFrame : IEquatable<InputFrame> {
    public InputType type { get; set; }
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

    public override string ToString() {
        return $"{type}#{frameType}";
    }
}

public class InputChord {
    public InputFrame[] inputs;
    public bool consumed { get; set; }

    public InputChord(params InputFrame[] inputs) {
        this.inputs = inputs;
    }

    public bool HasInput(EntitySide side, InputType type, InputFrameType frameType) {
        type = InputBuffer.TranslateToRawDirection(type, side);
        return inputs.Contains(new(type, frameType));
    }
    
    public bool HasInput(EntitySide side, InputFrame frame) {
        frame.type = InputBuffer.TranslateToRawDirection(frame.type, side);
        return inputs.Contains(frame);
    }

    public void Consume() {
        inputs = (from c in inputs where c.frameType == InputFrameType.HELD select c).ToArray();
        consumed = false;
    }

    public override string ToString() {
        return $"InputChord({string.Join(", ", inputs)})";
    }
}
}
