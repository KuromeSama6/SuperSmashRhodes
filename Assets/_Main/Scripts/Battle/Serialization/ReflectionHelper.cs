using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SuperSmashRhodes.Scripts.Util {
public static class ReflectionHelper {
    public static List<IField> GetAllFieldsAndPropertyBackingFields(Type type, Type stopAt = null) {
        var ret = new List<IField>();
        stopAt ??= typeof(MonoBehaviour);

        var searchType = type;
        while (searchType.BaseType != null && searchType != stopAt) {
            foreach (var field in searchType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)) {
                ret.Add(new FieldWrapper(field));
            }

            foreach (var prop in searchType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)) {
                ret.Add(new PropertyWrapper(prop));
            }

            searchType = searchType.BaseType;
        }
        
        return ret;
    }

    public static FieldWrapper GetBackingField(PropertyWrapper field, List<IField> fields = null) {
        if (fields == null) fields = GetAllFieldsAndPropertyBackingFields(field.prop.DeclaringType);
        return (FieldWrapper)fields.Find(c => c.name == $"<{field.prop.Name}>k__BackingField");
    }
}

public interface IField {
    Type type { get; }
    bool readable { get; }
    string name { get; }
    
    object GetValue(object obj);
    void SetValue(object obj, object value);
    string ToString();
    T GetAttribute<T>() where T : Attribute;
}

public class FieldWrapper : IField {
    public readonly FieldInfo field;
    public FieldWrapper(FieldInfo field) {
        this.field = field;
    }

    public Type type => field.FieldType;
    public bool readable => true;
    public string name => field.Name;

    public object GetValue(object obj) => field.GetValue(obj);
    public void SetValue(object obj, object value) => field.SetValue(obj, value);
    public T GetAttribute<T>() where T : Attribute => field.GetCustomAttribute<T>();
    public override string ToString() {
        return $"FieldWrapper({field})";
    }
}

public class PropertyWrapper : IField {
    public readonly PropertyInfo prop;
    public PropertyWrapper(PropertyInfo prop) {
        this.prop = prop;
    }

    public Type type => prop.PropertyType;
    public bool readable => true;
    public string name => prop.Name;
    public object GetValue(object obj) => prop.GetValue(obj);
    public void SetValue(object obj, object value) => prop.SetValue(obj, value);
    public T GetAttribute<T>() where T : Attribute => prop.GetCustomAttribute<T>();
    public override string ToString() {
        return $"PropertyWrapper({prop})";
    }
}

}
