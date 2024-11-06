using System;
using System.IO;
using SuperSmashRhodes.FScript;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace FScript.Editor {
[ScriptedImporter(1, "fscript")]
public class FScriptImporter : ScriptedImporter {

    public override void OnImportAsset(AssetImportContext ctx) {
        var content = File.ReadAllText(ctx.assetPath);

        FScriptObject obj = ScriptableObject.CreateInstance<FScriptObject>();
        obj.rawScript = content;
        
        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Main/Scripts/FScript/Resources/fscript-icon.psd");
        EditorGUIUtility.SetIconForObject(obj, icon);
        
        ctx.AddObjectToAsset("FScriptObject", obj);
        ctx.SetMainObject(obj);
    }
}
}
