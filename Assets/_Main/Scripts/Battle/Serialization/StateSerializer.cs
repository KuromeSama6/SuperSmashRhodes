using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SuperSmashRhodes.Battle.Serialization {
public class StateSerializer {
    public readonly SerializedDictionary objects = new();
    private readonly SerializerMode mode;
    private int deserializeCount;

    public StateSerializer() {
        mode = SerializerMode.SERIALIZE;
    }

    public StateSerializer(SerializedDictionary objects) {
        mode = SerializerMode.DESERIALIZE;
        this.objects = objects;
    }

    public void Serialize(string key, object obj, SerializationOption options = SerializationOption.NONE) {
        if (mode != SerializerMode.SERIALIZE) {
            throw new Exception("Cannot serialize in deserialize mode!");
        }

        if (obj != null && !CheckFieldType(obj.GetType())) {
            Debug.LogError($"Type {obj.GetType()} is not serializable! Consider converting it to a handle.");
            return;
        }

        if (objects.ContainsKey(key)) {
            throw new Exception($"Key {key} already exists in serializer!");
        }
        
        objects[key] = ToSerialized(obj, options);
    }
    
    public void SerializeReference(string key, object obj) {
        var direct = new DirectReferenceHandle(obj);
        Serialize(key, direct);
    }

    private object ToSerialized(object obj, SerializationOption options = SerializationOption.NONE) {
        if (obj == null) return null;
        if (obj is DirectReferenceHandle directReference) {
            return directReference;
        }
        
        // Debug.Log($"dct serialize: {obj.GetType()}");
        if (obj is IHandleSerializable handleSerializable && !options.HasFlag(SerializationOption.EXPAND)) {
            return ToSerialized(handleSerializable.GetHandle());
        }
        
        if (obj is IStateSerializable serializable) {
            var ser = new StateSerializer();
            serializable.Serialize(ser);

            if (obj is IHandle handle) {
                ser.Serialize("_handle", handle.GetType().FullName);
            }
            
            return ser.objects;
        }

        if (obj is IDictionary<string, object> dict) {
            Debug.Log(dict);
        }
        
        if (obj is IEnumerable<object> enumerable) {
            var list = new SerializedCollection();
            foreach (var s in enumerable) {
                list.Add(ToSerialized(s));
            }
            return list;
        }

        return obj;
    }

    public T Deserialize<T>(string key) {
        if (mode != SerializerMode.DESERIALIZE) {
            throw new Exception("Cannot deserialize in serialize mode!");
        }

        if (objects.Count == 0) {
            throw new Exception("No more objects to deserialize!");
        }

        if (!objects.ContainsKey(key)) {
            throw new Exception($"Key {key} not found in deserializer!");
        }

        object obj = objects[key];
        if (!(obj is T)) {
            throw new Exception($"Deserializer expected type {typeof(T)} but got {obj.GetType()} (#{deserializeCount})!");
        }

        objects.Remove(key);
        ++deserializeCount;
        return (T)obj;
    }
    
    public T DeserializeReference<T>(string key) {
        var handle = Deserialize<DirectReferenceHandle>(key);
        return (T)handle.GetObject();
    }
    
    public enum SerializerMode {
        SERIALIZE,
        DESERIALIZE
    }

    public static bool CheckFieldType(Type type) {
        // Debug.Log($"{type} {type.GetCustomAttribute<SerializableAttribute>() != null}");
        if (type.HasSerializationOption(SerializationOption.EXCLUDE)) {
            Debug.Log("returning false");
            return false;
        }
        if (type.HasSerializationOption(SerializationOption.DIRECT_REFERENCE)) return true;
        if (type.IsValueType || type == typeof(string) || typeof(IStateSerializable).IsAssignableFrom(type) || typeof(IHandleSerializable).IsAssignableFrom(type)) return true;

        if (typeof(IEnumerable).IsAssignableFrom(type) && type.GetGenericArguments().Length == 1) {
            var elementType = type.GetGenericArguments()[0];
            return CheckFieldType(elementType);
        }
        
        if (typeof(IDictionary).IsAssignableFrom(type) && type.GetGenericArguments().Length == 2) {
            var keyType = type.GetGenericArguments()[0];
            var valueType = type.GetGenericArguments()[1];
            return (keyType.IsValueType || keyType == typeof(string)) && CheckFieldType(valueType);
        }
        
        return false;
    }
}

[SerializationOptions]
public class SerializedDictionary : Dictionary<string, object> { }

[SerializationOptions]
public class SerializedCollection : List<object> { }
}
