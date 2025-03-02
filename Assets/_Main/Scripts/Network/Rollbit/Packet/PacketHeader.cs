using System;
using SuperSmashRhodes.Util;

namespace SuperSmashRhodes.Network.Rollbit {
public class PacketHeader {
    public static readonly ushort ROLLBIT_MAGIC = 0x7940;
    public static readonly ushort P2P_MAGIC = 0x0721;
    
    public PacketType type {get; private set;}
    public ushort magic {get; private set;}
    public ushort version {get; private set;}
    public uint length {get; set;}
    public uint bodyLength {get; private set;}
    public ushort statusCode {get; private set;}
    public uint requestId {get; set;}
    public string userId {get; private set;}

    public ByteBuf bytes {
        get {
            var ret = new ByteBuf(32);
            ret.SetWordAt(0, magic);
            ret.SetWordAt(2, version);
            ret.SetWordAt(4, (ushort)type);
            ret.SetDWordAt(6, length);
            ret.SetWordAt(10, statusCode);
            ret.SetDWordAt(12, requestId);
            ret.SetString(16, userId, 16);
            return ret;
        }
    }
    
    public PacketHeader(ByteBuf buf) {
        magic = buf.GetWordAt(0);
        version = buf.GetWordAt(2);
        type = (PacketType)buf.GetWordAt(4);
        length = buf.GetDWordAt(6);
        bodyLength = length - 32;
        statusCode = buf.GetWordAt(10);
        requestId = buf.GetDWordAt(12);
        userId = buf.GetStringNT(16, 16);
    }

    public PacketHeader(PacketType type, ushort magic, ushort version, uint length, uint bodyLength, ushort statusCode, uint requestId, string userId) {
        this.magic = magic;
        this.type = type;
        this.version = version;
        this.length = length;
        this.bodyLength = bodyLength;
        this.statusCode = statusCode;
        this.requestId = requestId;
        this.userId = userId;
    }

    public override string ToString() {
        return $"PacketHeader(type={type}({type:X}), version={version:X}, length={length}, statusCode={statusCode:X}, requestId={requestId}, userId={userId})";
    }
}


}
