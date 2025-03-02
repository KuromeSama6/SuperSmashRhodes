using System;
using System.Buffers.Binary;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SuperSmashRhodes.Util {
public class ByteBuf {
    private readonly byte[] buf;

    public uint size => (uint)buf.Length;
    public byte[] bytes => buf.ToArray();
    
    public ByteBuf(uint size) : this(new byte[size]) {
        
    }

    public ByteBuf(byte[] buf) {
        this.buf = buf.ToArray();
    }

    public byte[] GetBytes(int start, int length) {
        return buf[start..(start + length)];
    }
    
    public byte[] GetBytesBetween(int start, int end) {
        // Debug.Log($"getting bytes between {start} and {end}");
        return buf[start..end];
    }

    public void SetBytes(int start, ByteBuf buf) {
        SetBytes(start, buf.bytes);
    }
    public void SetBytes(int start, byte[] bytes) {
        for (int i = 0; i < bytes.Length; i++) {
            buf[start + i] = bytes[i];
        }
    }
    
    public byte GetByteAt(int at) {
        return buf[at];
    }
    public ushort GetWordAt(int at) {
        return BinaryPrimitives.ReadUInt16BigEndian(buf[at..(at + 2)]);
    }
    public uint GetDWordAt(int at) {
        return BinaryPrimitives.ReadUInt32BigEndian(buf[at..(at + 4)]);
    }
    public ulong GetQWordAt(int at) {
        return BinaryPrimitives.ReadUInt64BigEndian(buf[at..(at + 8)]);
    }
    public string GetStringNT(int at, int maxLength) {
        var bytes = GetBytes(at, maxLength);
        // parse to string
        var str = Encoding.UTF8.GetString(bytes);
        var parts = str.Split('\0');
        
        return parts.Length > 0 ? parts[0] : str;
    }
    
    public void SetByteAt(int at, byte value) {
        buf[at] = value;
    }
    public void SetWordAt(int at, ushort value) {
        BinaryPrimitives.WriteUInt16BigEndian(buf.AsSpan(at), value);
    }
    public void SetDWordAt(int at, uint value) {
        BinaryPrimitives.WriteUInt32BigEndian(buf.AsSpan(at), value);
    }
    public void SetQWordAt(int at, ulong value) {
        BinaryPrimitives.WriteUInt64BigEndian(buf.AsSpan(at), value);
    }
    public void SetStringNT(int at, string str) {
        if (str != null) {
            var bytes = Encoding.UTF8.GetBytes(str);
            SetBytes(at, bytes);
        }
        
        SetByteAt(at + (str != null ? str.Length : 0), 0x0);
    }
    
    public void SetString(int at, string str, int maxLength) {
        if (str != null) {
            var bytes = Encoding.UTF8.GetBytes(str);
            SetBytes(at, bytes);
        }
        
        for (int i = str != null ? str.Length : 0; i < maxLength; i++) {
            SetByteAt(at + i, 0x0);
        }
    }

    public ByteBuf Copy() {
        return new(buf);
    }

    public override string ToString() {
        return $"ByteBuf[{size}]({string.Join(", ", buf.Select(b => b.ToString("X2")))})";
    }

}
}
