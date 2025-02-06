using System;
using System.Reflection;

namespace SuperSmashRhodes.Network.Rollbit {
public interface IPacketHandler { }

[AttributeUsage(AttributeTargets.Method)]
public class PacketHandlerAttribute : Attribute {}

public struct RegisteredPacketHandler {
    public Type handlerClass;
    public IPacketHandler handler;
    public MethodInfo method;

    public RegisteredPacketHandler(Type handlerClass, IPacketHandler handler, MethodInfo info) {
        this.handlerClass = handlerClass;
        this.handler = handler;
        method = info;
    }

    public void Invoke(ClientboundPacket packet) {
        method.Invoke(handler, new object[] {packet});
    }

    public override string ToString() {
        return $"RegisteredPacketHandler({handlerClass}, {handler}, {method})";
    }

}

}
