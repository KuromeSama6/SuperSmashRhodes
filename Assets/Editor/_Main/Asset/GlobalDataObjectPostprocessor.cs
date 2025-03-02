using System;
using System.Reflection;
using SuperSmashRhodes.Extensions;
using SuperSmashRhodes.Framework;
using UnityEditor;
using UnityEngine;

namespace SuperSmashRhodes.Editor.Asset {
public class GlobalDataObjectPostprocessor : AssetPostprocessor {
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
        foreach (var type in Assembly.GetAssembly(typeof(GlobalDataObject<>)).GetTypes()) {
            if (type.IsSubclassOfGeneric(typeof(GlobalDataObject<>)) && !type.IsAbstract) {
                // type.GetProperty("inst", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).GetValue(null);
            }
        }
    }
}
}
