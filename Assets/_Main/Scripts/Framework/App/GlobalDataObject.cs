using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SuperSmashRhodes.Framework {
public abstract class GlobalDataObject<T> : ScriptableObject where T : ScriptableObject {
    public static T inst {
        get {
            if (_inst) return _inst;
            // create instance
            var fileName = $"{typeof(T).Name}_GlobalData";
            var path = $"DataObjects/Global/{fileName}";

            _inst = Resources.Load<T>(path);
            if (!_inst) {
                Debug.LogWarning($"GlobalDataObject({path}) not found in Resources/Global. Creating it.");
                var obj = CreateInstance<T>();
                obj.name = fileName;
                SaveAsset(obj, $"Assets/_Main/Resources/{path}.asset");
                _inst = obj;
            }
            return _inst;
        }
    }

    private static T _inst;
    
    public void SaveNow() {
        #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        #endif
    }
    
    private static void SaveAsset(T config, string path) {
    #if UNITY_EDITOR
        if (!Directory.Exists("Assets/Resources/_Main")) {
            Directory.CreateDirectory("Assets/Resources/_Main");
        }

        AssetDatabase.CreateAsset(config, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Created new GlobalConfig at " + path);
    #else
        Debug.LogError("Cannot create GlobalConfig at runtime in a build! Ensure it's included in Resources.");
    #endif
    }
}
}
