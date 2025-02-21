using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuperSmashRhodes.Battle.Serialization {
/// <summary>
/// Represents that the implementing class can be serialized and deserialized.
/// This is the most basic form of serialization.
/// </summary>
public interface IStateSerializable {
    void Serialize(StateSerializer serializer);
    void Deserialize(StateSerializer serializer);
}

/// <summary>
/// Represents that the implementing class can be serialized and deserialized by converting it to a <code>IHandle</code>.
/// </summary>
public interface IHandleSerializable {
    IHandle GetHandle();
}

}
