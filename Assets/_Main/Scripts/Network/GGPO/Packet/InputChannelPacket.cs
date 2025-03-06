using System;
using System.Collections.Generic;
using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.GGPOWrapper.Packet {
/// <summary>
/// Represents an abstraction of an input packet send through GGPO.
/// Since GGPO hides the UDP transportation layer, we can't send packets on-demand - we must send one packet per frame.
/// An input channel packet combines multiple subpackets - each containing some data. For instance, one of the subpackets may contain the player's input. Other packets can be used to transmit other auxiliary data.
/// </summary>
public class InputChannelPacket {
    public readonly static byte MAGIC = 0x80;
    private readonly static int MAX_SIZE = GGPOConnector.INPUT_BUFFER_SIZE;
    
    public readonly List<ChannelSubpacket> subpackets = new();
    public int playerId { get; private set; }
    
    public InputChannelPacket(int playerId, params ChannelSubpacket[] subpackets) {
        this.subpackets.AddRange(subpackets);
        this.playerId = playerId;
    }

    public T FindFirst<T>() where T : ChannelSubpacket {
        return (T)subpackets.Find(p => p is T);
    }
    
    public InputChannelPacket(ByteBuf buf) {
        if (buf.GetByteAt(0) != MAGIC) {
            throw new ArgumentException($"Invalid magic byte: {buf.GetByteAt(0)}");
        }
        
        playerId = buf.GetByteAt(1);
        var subpacketCount = buf.GetWordAt(2);
        
        int index = 4;
        for (int i = 0; i < subpacketCount; i++) {
            var type = (SubpacketType)buf.GetWordAt(index);
            var size = buf.GetWordAt(index + 2);
            var packet = ChannelSubpacket.CreateFromType(type, buf.Slice(index + 4, size));
            subpackets.Add(packet);
            
            index += size + 2 + 2;
        }
        
    }
    
    /// <summary>
    /// Serializes the packet into a byte array. The structure of the packet is as follows:
    /// 0: Magic 0x80
    /// 1: Player ID (The id of the player from which this packet is sent)
    /// 2-3: Subpacket count
    /// </summary>
    /// <returns>The serialized data.</returns>
    public byte[] Serialize() {
        var buf = new ByteBuf(MAX_SIZE);
        buf.SetByteAt(0, MAGIC);
        buf.SetByteAt(1, (byte)playerId);
        buf.SetWordAt(2, (ushort)subpackets.Count);

        // add subpackets
        int index = 4;
        foreach (var packet in subpackets) {
            buf.SetWordAt(index, (ushort)packet.type);
            
            var size = packet.size;
            buf.SetWordAt(index + 2, (ushort)size);
            
            buf.SetBytes(index + 4, packet.Serialize());
            
            index += size + 2 + 2;
        }
        
        return buf.bytes;
    }
    
}
}
