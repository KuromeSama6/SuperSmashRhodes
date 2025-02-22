using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Scripts.Util;
using Unity.VisualScripting;
using UnityEngine;

namespace SuperSmashRhodes.Battle.Serialization {
public class ReflectionSerializer : IStateSerializable {
    private static readonly Dictionary<Type, Dictionary<string, IField>> fieldsCache = new();
    private static readonly Dictionary<Type, Dictionary<string, IField>> allFieldsCache = new();
    private static readonly Dictionary<string, Type> typeCache = new();
    
    private readonly object targetObject;
    private readonly Dictionary<string, IField> fields = new();
    private readonly Dictionary<string, IField> allFields = new();

    public ReflectionSerializer(object target) {
        targetObject = target;
        var type = target.GetType();
        typeCache[type.FullName] = type;

        if (fieldsCache.TryGetValue(type, out var cachedFields)) {
            fields.AddRange(cachedFields);
            allFields.AddRange(allFieldsCache[type]);
            
        } else {
            // register fields
            allFields = ReflectionHelper.GetAllFieldsAndPropertyBackingFields(type);
            allFieldsCache[type] = allFields;
            
            foreach (var (fieldName, field) in allFields) {
                // Debug.Log($"checking field {field} on {type}, field is property: {field is PropertyWrapper}");
                if (field.type == GetType()) continue;
                if (field.GetAttribute<CompilerGeneratedAttribute>() != null) continue;
                fields[field.name] = field;
                typeCache[type.FullName] = field.type;
            }   
            
            fieldsCache[type] = fields;
        }
        
        // print all property names in one line
        // Debug.Log($"Fields ({obj.GetType()}) ({fields.Count}): {string.Join(", ", fields)}");
    }

    public void Serialize(StateSerializer serializer) {
        foreach (var (fieldName, field) in fields) {
            IField backingField = field is PropertyWrapper prop ? ReflectionHelper.GetBackingField(prop, allFields) : field;
            if (backingField == null) continue;
            
            var fieldValue = backingField.GetValue(targetObject);
            
            var options = field.GetAttribute<SerializationOptions>();
            var flags = options?.options ?? SerializationOption.NONE;
            var path = $"$${backingField.name}";
            if (flags.HasFlag(SerializationOption.EXCLUDE)) continue;
            
            
            object finalValue;
            if (flags.HasFlag(SerializationOption.DIRECT_REFERENCE)) {
                // direct reference handle
                finalValue = new DirectReferenceHandle(fieldValue);
                
            } else if (field.type == typeof(SerializedDictionary) || field.type == typeof(SerializedCollection)) {
                finalValue = fieldValue;

            } else if (StateSerializer.CheckFieldType(field.type)) {
                finalValue = fieldValue;
                
            } else if (fieldValue is IDictionary dictionary && field.type.GetGenericArguments().Length == 2) {
                var dict = new Dictionary<object, object>();
                foreach (var key in dictionary.Keys) {
                    dict[key] = dictionary[key];
                }
                
                serializer.PutDict(path, dict);
                continue;
                
            } else {
                // Debug.LogWarning($"Serialization skipped for {field} on {targetObject.GetType()}. Consider converting it to a handle or marking it as direct reference.");
                continue;
            }
            
            // serializer.Serialize($"_reflection${obj.GetType()}::{backingField.name}", finalValue);
            serializer.Put(path, finalValue, flags);
        }

        // serializer.Serialize("_reflection_class", obj.GetType().FullName);
    }

    public void Deserialize(StateSerializer serializer) {
        foreach (var (fieldName, field) in fields) {
            DeserializeField(serializer, field);
        }
    }

    private void DeserializeField(StateSerializer serializer, IField field) {
        var objectType = field.type;
        IField backingField = field is PropertyWrapper prop ? ReflectionHelper.GetBackingField(prop, allFields) : field;
        if (backingField == null) return;
        var options = field.GetAttribute<SerializationOptions>();
        var flags = options?.options ?? SerializationOption.NONE;
        if (flags.HasFlag(SerializationOption.EXCLUDE)) return;
        if (!StateSerializer.CheckFieldType(objectType)) return;

        var path = $"$${backingField.name}";
        var value = serializer.Get<object>(path);
        
        {
            if (value is DirectReferenceHandle directReferenceHandle) { 
                var deserialized = directReferenceHandle.Resolve();
                
                if (deserialized is IDictionary<object, object>) {
                    object fieldValue = backingField.GetValue(targetObject);
                    if (fieldValue is IDictionary<object, object> targetDict) {
                        serializer.GetDict(path, targetDict);
                        return;
                        
                    } else {
                        Debug.LogWarning("Could not deserialize dictionary into non-dictionary field.");
                    }
                }
                
                backingField.SetValue(targetObject, deserialized);
                return;
            }   
        }

        {
         
            if (value is IHandle handle) {
                backingField.SetValue(targetObject, handle.Resolve());
                return;
            }   
        }
        
        {
            if (value is SerializedDictionary dict) {
                var ser = new StateSerializer(dict);
                // Debug.Log($"dict, $${backingField.name}, {ser.objects.parentType}");

                object fieldValue = backingField.GetValue(targetObject);
                if (ser.objects.ContainsKey("_handle")) {
                    var objectHandle = ser.Get<IHandle>("_handle");
                    var handleValue = objectHandle.Resolve();

                    // Debug.Log("has handle");
                    if (fieldValue.GetType() != handleValue.GetType()) {
                        // Debug.Log($"type mismatch, setting. Field type {fieldValue.GetType()}, handle {handleValue.GetType()}");
                        backingField.SetValue(targetObject, handleValue);
                        fieldValue = handleValue;
                        
                    } else {
                        // Debug.Log("field type ok, skipping");
                    }

                }

                if (fieldValue is IStateSerializable stateSerializable) {
                    stateSerializable.Deserialize(ser);
                }
                return;
            }   
        }
        
        {
            if (value is SerializedCollection collection) {
                if (!(backingField.GetValue(targetObject) is IList list)) {
                    throw new Exception($"Could not deserialize collection {backingField.name} on {targetObject.GetType()}.");
                }
            
                list.Clear();
                foreach (var rawItem in collection) {
                    if (rawItem is DirectReferenceHandle directReferenceHandle) {
                        list.Add(directReferenceHandle.Resolve());
                        continue;
                    }
                    
                    if (rawItem is IHandle handle) {
                        // Debug.Log($"add handle {rawItem}");
                        list.Add(handle.Resolve());
                        continue;
                    }

                    if (rawItem is SerializedDictionary || rawItem is SerializedCollection) {
                        throw new Exception($"Nested collections within a list are not supported. Consider using a direct reference handle or manual deserialization. (Field: {backingField.name}, Type: {targetObject.GetType()}, rawItem: {rawItem})");
                    }
                
                    list.Add(rawItem);
                }
            
                return;
            }   
        }
        
        backingField.SetValue(targetObject, value);
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