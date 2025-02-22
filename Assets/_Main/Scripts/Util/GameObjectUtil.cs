using System.Collections.Generic;
using UnityEngine;

namespace SuperSmashRhodes.Util {
public static class GameObjectUtil {
    public static string GetAbsolutePath(this GameObject go) {
        var ret = new Stack<string>();
        var transform = go.transform;
        while (transform != null) {
            ret.Push(transform.name);
            transform = transform.parent;
        }
        
        return string.Join("/", ret);
    }

    public static GameObject FindByAbsolutePath(string path) {
        var parts = path.Split('/');
        GameObject current = null;
        
        for (int i = 0; i < parts.Length; i++) {
            var part = parts[i];
            var child = current == null ? GameObject.Find(part) : current.transform.Find(part)?.gameObject;
            if (child == null) {
                return null;
            } else {
                current = child;
            }
        }
        
        return current;
    }

    public static GameObject CreateByAbsolutePath(string path) {
        var parts = path.Split('/');
        GameObject current = null;
        
        for (int i = 0; i < parts.Length; i++) {
            var part = parts[i];
            var child = current == null ? GameObject.Find(part) : current.transform.Find(part)?.gameObject;
            if (child == null) {
                child = new GameObject(part);
                if (current != null) {
                    child.transform.SetParent(current.transform);
                }
                current = child;
            } else {
                current = child;
            }
        }
        
        return current;
    }

    public static GameObject FindOrCreateByAbsolutePath(string path) {
        return FindByAbsolutePath(path) ?? CreateByAbsolutePath(path);
    }
}
}
