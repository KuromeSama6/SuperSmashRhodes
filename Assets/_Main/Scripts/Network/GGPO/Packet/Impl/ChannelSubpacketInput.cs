using SuperSmashRhodes.Input;
using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.GGPOWrapper.Packet {
public class ChannelSubpacketInput : ChannelSubpacket {
    public InputChord inputChord { get; private set; }

    public ChannelSubpacketInput(InputChord inputChord) {
        this.inputChord = inputChord;
    }
    
    public ChannelSubpacketInput(ByteBuf buf) : base(buf) {
        inputChord = new InputChord(buf.bytes);
    }

    public override SubpacketType type => SubpacketType.INPUT;
    public override int size => inputChord.serializedSize;

    protected override void SerializeInternal(ByteBuf buf) {
        buf.SetBytes(0, inputChord.Serialize(0));
    }
}
}
