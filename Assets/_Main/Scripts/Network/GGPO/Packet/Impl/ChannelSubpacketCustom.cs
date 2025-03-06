using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.GGPOWrapper.Packet {
public class ChannelSubpacketCustom : ChannelSubpacket {
    private static int nonceCounter = 0;
    
    public override SubpacketType type => SubpacketType.AUXILIARY_CUSTOM;
    public override int size => 4 + data.Length;
    public readonly int nonce;
    private readonly byte[] data;

    public ByteBuf buffer => new ByteBuf(data);
    
    public ChannelSubpacketCustom(byte[] data) {
        this.data = data;
        nonce = nonceCounter++;
    }
    
    public ChannelSubpacketCustom(ByteBuf buf) : base(buf) {
        nonce = buf.GetWordAt(0);
        data = buf.GetBytes(4, (int)buf.size - 4);
    }
    
    protected override void SerializeInternal(ByteBuf buf) {
        buf.SetDWordAt(0, (uint)nonce);
        buf.SetBytes(4, data);
    }
}
}
