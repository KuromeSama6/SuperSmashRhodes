using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketPlayOutBeginP2P : ClientboundPacket {
    public readonly bool useP2P;
    public readonly byte[] peerAddress;
    public readonly int peerPort;
    public readonly ulong nonce, verifier, expectedVerifier;
    public readonly string aesKey;
    
    public string peerAddressString => string.Join(".", peerAddress);
    
    public PacketPlayOutBeginP2P(NetworkSession session, PacketHeader header, ByteBuf body) : base(session, header, body) {
        useP2P = body.GetByteAt(0) == 1;
        peerAddress = body.GetBytes(2, 4);
        peerPort = body.GetWordAt(6);
        
        nonce = body.GetQWordAt(8);
        verifier = body.GetQWordAt(16);
        expectedVerifier = body.GetQWordAt(24);
        aesKey = useP2P ? body.GetStringNT(32, 16) : null;
    }

    public override PacketType type => PacketType.PLAY_OUT_BEGIN_P2P;

    public override string ToString() {
        return $"PacketPlayOutBeginP2P(useP2P={useP2P}, peerAddress={string.Join(".", peerAddress)}, peerPort={peerPort}, nonce={nonce}, verifier={verifier}, expectedVerifier={expectedVerifier}, aesKey={aesKey})";
    }
}
}
