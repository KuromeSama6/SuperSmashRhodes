using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SuperSmashRhodes.Battle.Serialization {
public class StateSerializer {
    public readonly SerializedDictionary objects;
    
    public StateSerializer(Type parentType = null) {
        objects = new (parentType);
    }

    public StateSerializer(SerializedDictionary objects) {
        this.objects = objects;
    }

    public void Put(string key, object obj, SerializationOption options = SerializationOption.NONE) {
        if (obj is SerializedDictionary || obj is SerializedCollection) {
            objects[key] = obj;
            return;
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
    
    public void PutReference(string key, object obj) {
        var direct = new DirectReferenceHandle(obj);
        Put(key, direct);
    }

    public void PutDict<TKey, TValue>(string key, IDictionary<TKey, TValue> dict) {
        PutReference(key, new Dictionary<TKey, TValue>(dict));
    }
    
    public void PutList<T>(string key, IList<T> list) {
        PutReference(key, new List<T>(list));
    }

    public T Get<T>(string key) {
        if (objects.Count == 0) {
            throw new Exception("No more objects to deserialize!");
        }

        if (!objects.ContainsKey(key)) {
            throw new Exception($"Key {key} not found in deserializer!");
        }

        object obj = objects[key];
        return (T)obj;
    }
    
    public T GetReference<T>(string key) {
        var handle = Get<DirectReferenceHandle>(key);
        return (T)handle.Resolve();
    }

    public void GetList<T>(string key, IList<T> outputList) {
        var list = GetReference<List<T>>(key);
        outputList.Clear();
        foreach (var item in list) {
            outputList.Add(item);
        }
    }
    
    public void GetDict<TKey, TValue>(string key, IDictionary<TKey, TValue> outputDict) {
        var dict = GetReference<IDictionary<TKey, TValue>>(key);
        outputDict.Clear();
        foreach (var (k, v) in dict) {
            outputDict[k] = v;
        }
    }

    public T GetHandle<T>(string key) {
        return (T)Get<IHandle>(key).Resolve();
    }

    public StateSerializer GetObject(string key) {
        return new StateSerializer(Get<SerializedDictionary>(key));
    }
    
    private object ToSerialized(object obj, SerializationOption options = SerializationOption.NONE) {
        if (obj == null) return null;
        if (obj is DirectReferenceHandle directReference) {
            return directReference;
        }
        
        // Debug.Log($"dct serialize: {obj.GetType()}");
        if (obj is IHandleSerializable handleSerializable && !options.HasFlag(SerializationOption.EXPAND)) {
            return handleSerializable.GetHandle();
        }
        
        if (obj is IHandle handle) {
            return handle;
        }
        
        if (obj is IStateSerializable serializable) {
            var ser = new StateSerializer(serializable.GetType());
            serializable.Serialize(ser);

            if (obj is IHandleSerializable handleSer) {
                ser.Put("_handle", handleSer.GetHandle());
            }
            
            return ser.objects;
        }

        if (obj is IDictionary<string, object> dict) {
            Debug.Log(dict);
        }
        
        if (obj is IEnumerable<object> enumerable) {
            var list = new SerializedCollection(enumerable.GetType());
            foreach (var s in enumerable) {
                list.Add(ToSerialized(s));
            }
            return list;
        }

        return obj;
    }
    
    public static bool CheckFieldType(Type type) {
        // Debug.Log($"{type} {type.GetCustomAttribute<SerializableAttribute>() != null}");
        if (type.HasSerializationOption(SerializationOption.EXCLUDE)) {
            return false;
        }
        if (type.HasSerializationOption(SerializationOption.DIRECT_REFERENCE)) return true;
        if (type.IsValueType || type == typeof(string) || typeof(IStateSerializable).IsAssignableFrom(type) || typeof(IHandleSerializable).IsAssignableFrom(type)) return true;

        if (typeof(IEnumerable).IsAssignableFrom(type) && type.GetGenericArguments().Length == 1) {
            var elementType = type.GetGenericArguments()[0];
            return CheckFieldType(elementType);
        }
        
        // if (typeof(IDictionary).IsAssignableFrom(type) && type.GetGenericArguments().Length == 2) {
        //     var keyType = type.GetGenericArguments()[0];
        //     var valueType = type.GetGenericArguments()[1];
        //     return (keyType.IsValueType || keyType == typeof(string)) && CheckFieldType(valueType);
        // }
        
        return false;
    }
}

[SerializationOptions]
public class SerializedDictionary : Dictionary<string, object> {
    public readonly Type parentType;
    public SerializedDictionary(Type parentType) {
        this.parentType = parentType;
    }
}

[SerializationOptions]
public class SerializedCollection : List<object> {
    public readonly Type parentType;
    public SerializedCollection(Type parentType) {
        this.parentType = parentType;
    }
}
}
