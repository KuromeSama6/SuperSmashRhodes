using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using SuperSmashRhodes.Scripts.Util;
using Unity.VisualScripting;
using UnityEngine;

namespace SuperSmashRhodes.Battle.Serialization {
public class ReflectionSerializer : IStateSerializable {
    private static readonly Dictionary<Type, List<IField>> cache = new();
    
    private readonly object obj;
    private readonly List<IField> fields = new();

    public ReflectionSerializer(object target) {
        obj = target;
        var type = target.GetType();

        if (cache.TryGetValue(type, out var cachedFields)) {
            fields.AddRange(cachedFields);
            
        } else {
            // register fields
            var allFields = ReflectionHelper.GetAllFieldsAndPropertyBackingFields(type);
            foreach (var field in allFields) {
                // Debug.Log($"checking field {field} on {type}, field is property: {field is PropertyWrapper}");
                if (field.type == GetType()) continue;
                if (field.GetAttribute<CompilerGeneratedAttribute>() != null) continue;
                fields.Add(field);
            }   
            
            cache[type] = fields;
        }
        
        // print all property names in one line
        // Debug.Log($"Fields ({obj.GetType()}) ({fields.Count}): {string.Join(", ", fields)}");
    }

    public void Serialize(StateSerializer serializer) {
        foreach (var field in fields) {
            IField backingField = field is PropertyWrapper prop ? ReflectionHelper.GetBackingField(prop) : field;
            if (backingField == null) continue;
            
            var fieldValue = backingField.GetValue(obj);
            
            var options = field.GetAttribute<SerializationOptions>();
            var flags = options?.options ?? SerializationOption.NONE;
            // Debug.Log(flags);
            
            if (flags.HasFlag(SerializationOption.EXCLUDE)) continue;

            object finalValue;
            if (flags.HasFlag(SerializationOption.DIRECT_REFERENCE)) {
                // direct reference handle
                finalValue = new DirectReferenceHandle(fieldValue);
                
            } else if (StateSerializer.CheckFieldType(field.type)) {
                finalValue = fieldValue;
                
            } else {
                Debug.LogWarning($"Serialization skipped for {field} on {obj.GetType()}. Consider converting it to a handle or marking it as direct reference.");
                continue;
            }
            
            // serializer.Serialize($"_reflection${obj.GetType()}::{backingField.name}", finalValue);
            serializer.Serialize($"$${backingField.name}", finalValue, flags);
        }
    }

    public void Deserialize(StateSerializer serializer) {
        throw new System.NotImplementedException();
    }
}

public interface IReflectionSerializable : IStateSerializable {
    ReflectionSerializer reflectionSerializer { get; }
    
    void IStateSerializable.Serialize(StateSerializer serializer) {
        reflectionSerializer.Serialize(serializer);
    }
    void IStateSerializable.Deserialize(StateSerializer serializer) {
        reflectionSerializer.Deserialize(serializer);
    }
}



[AttributeUsage(AttributeTargets.Class | AttributeTargets.Delegate | AttributeTargets.Interface | AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
public class SerializationOptions : Attribute {
    public readonly SerializationOption options;
    public SerializationOptions(SerializationOption options = SerializationOption.NONE) {
        this.options = options;
    }
}


[Flags]
public enum SerializationOption {
    NONE = 0,
    /// <summary>
    /// Exclude this field from serialization.
    /// </summary>
    EXCLUDE = 1 << 0,
    /// <summary>
    /// Serialize this field as a direct reference.
    /// </summary>
    DIRECT_REFERENCE = 1 << 1,
    /// <summary>
    /// Serialize this field with all of its properties expanded.
    /// The field must implement IStateSerializable.
    /// </summary>
    EXPAND = 1 << 2
}

public static class SerializationFlagExtensions {
    public static bool HasSerializationOption(this Type type, SerializationOption option) {
        var attr = type.GetCustomAttribute<SerializationOptions>();
        if (attr == null) return false;
        
        return (attr.options & option) != 0;
    }
    
    public static bool HasSerializationOption(this IField field, SerializationOption option) {
        var attr = field.GetAttribute<SerializationOptions>();
        if (attr == null) return false;
        
        return (attr.options & option) != 0;
    }
}
}