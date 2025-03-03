using System.Linq;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Network.Rollbit.P2P;
using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayP2PInput : P2PPacket {
    public static readonly ushort PAYLOAD_SIZE = 2;
    
    public int frameNumber { get; private set; }
    public InputFrame[] frames { get; private set; }
    
    public PacketPlayP2PInput(P2PConnector connector, PacketHeader header, ByteBuf body) : base(connector, header, body) {
        frameNumber = (int)body.GetDWordAt(0);
        var length = (int)body.GetDWordAt(4);
        frames = new InputFrame[length];
        
        for (int i = 0; i < length; i++) {
            var start = 10 + i * PAYLOAD_SIZE;
            var inputType = (InputType)body.GetByteAt(start);
            var frameType = (InputFrameType)body.GetByteAt(start + 1);
            frames[i] = new(inputType, frameType);
        }
        
    }
    
    public PacketPlayP2PInput(P2PConnector connector, int frameNumber, InputFrame[] frames) : base(connector) {
        this.frameNumber = frameNumber;
        this.frames = frames;
    }

    public override PacketType type => PacketType.PLAY_P2P_INPUT;
    public override uint bodySize => (uint)(4 + 4 + 2 + frames.Length * PAYLOAD_SIZE);

    public NetworkInputFrame ToInputFrame() {
        return new(frameNumber, frames.ToArray());
    }

    protected override void SerializeInternal(ByteBuf buf) {
        buf.SetDWordAt(0, (uint)frameNumber);
        buf.SetDWordAt(4, (uint)frames.Length);
        buf.SetWordAt(8, PAYLOAD_SIZE);
        
        for (int i = 0; i < frames.Length; i++) {
            var start = 10 + i * PAYLOAD_SIZE;
            buf.SetByteAt(start, (byte)frames[i].type);
            buf.SetByteAt(start + 1, (byte)frames[i].frameType);
        }
    }
}
}
