using System;
using SuperSmashRhodes.Util;
using Unity.VisualScripting;

namespace SuperSmashRhodes.GGPOWrapper.Packet {
public abstract class ChannelSubpacket {
    public ChannelSubpacket() {}
    public ChannelSubpacket(ByteBuf buf) {
    }
    
    public abstract SubpacketType type { get; }
    public abstract int size { get; }

    public byte[] Serialize() {
        var buf = new ByteBuf(size);
        SerializeInternal(buf);
        return buf.bytes;
    }

    protected abstract void SerializeInternal(ByteBuf buf);
    
    public static ChannelSubpacket CreateFromType(SubpacketType type, ByteBuf buf) {
        switch (type) {
            case SubpacketType.INPUT:
                return new ChannelSubpacketInput(buf);
            case SubpacketType.AUXILIARY_CUSTOM:
                return new ChannelSubpacketCustom(buf);
            default:
                throw new ArgumentException($"Unknown subpacket type: {type}");
        }
    }

    public override string ToString() {
        return $"ChannelSubpacket(type={type}, size={size}, data={Serialize().ToHexString()})";
    }
}
}
