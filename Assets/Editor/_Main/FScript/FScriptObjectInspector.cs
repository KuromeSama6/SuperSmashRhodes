using System;
using SuperSmashRhodes;
using SuperSmashRhodes.FScript;
using UnityEditor;
using UnityEngine;

namespace FScript.Editor {
[CustomEditor(typeof(FScriptObject))]
public class FScriptObjectInspector : UnityEditor.Editor {
    private bool loaded = false;

    private void OnEnable() {
        FInstructionRegistry.Scan();
        loaded = false;
    }

    private void OnDisable() {
        loaded = false;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        serializedObject.Update();
        FScriptObject fScript = (FScriptObject)target;
        EditorGUI.EndDisabledGroup();
        if (GUILayout.Button("Reload")) {
            loaded = false;
            FInstructionRegistry.Scan();
            Repaint();
        }
        
        if (!loaded) {
            if (!Load(fScript)) return;
            loaded = true;
        }
        
        // warnings
        if (fScript.descriptor == null)
            EditorGUILayout.HelpBox("FScript is missing .data block", MessageType.Error);
        else if (fScript.descriptor.invalidMessage != null)
            EditorGUILayout.HelpBox("FScript .data is invalid: " + fScript.descriptor.invalidMessage, MessageType.Error);
        
        if (fScript.inputMethod == null)
            EditorGUILayout.HelpBox("FScript is missing .input block", MessageType.Error);
        else if (fScript.inputMethod.invalidMessage != null)
            EditorGUILayout.HelpBox("FScript .input is invalid: " + fScript.inputMethod.invalidMessage, MessageType.Error);
        
        if (!fScript.blocks.ContainsKey("main"))
            EditorGUILayout.HelpBox("FScript does not have .main block", MessageType.Error);
        
        // properties
        if (fScript.descriptor != null) {
            EditorGUILayout.LabelField("Script Properties", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("ID", fScript.descriptor.id, EditorStyles.boldLabel);  
            EditorGUILayout.LabelField("Pretty Name", fScript.descriptor.prettyName, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Blocks", Mathf.Max(0, fScript.blocks.Count - 2).ToString(), EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Subroutines", Mathf.Max(0, fScript.blocks.Count - 3).ToString(), EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Max Input Buffer", $"{fScript.descriptor.bufferFrames}F", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Scan Priority", $"{fScript.descriptor.priority}", EditorStyles.boldLabel);
            
        }

        if (fScript.main != null) {
            EditorGUILayout.LabelField("Frame Data", EditorStyles.boldLabel);
            var overview = fScript.main.overview;
            EditorGUILayout.LabelField("Startup", $"{overview.startup}F", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Active", $"{overview.active}F", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Recovery", $"{overview.recovery}F", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Total", $"{overview.total}F", EditorStyles.boldLabel);
            
        }
        
        foreach (var block in fScript.blocks.Values) {
            if (block.isEmpty) EditorGUILayout.HelpBox($"{block.label}: Block is empty", MessageType.Warning);
        }
        
        // EditorGUILayout.LabelField("Raw Content");
        // EditorGUI.BeginDisabledGroup(true);
        // EditorGUILayout.TextArea(fScript.rawScript);
        // EditorGUI.EndDisabledGroup();
    }

    private bool Load(FScriptObject fScript) {
        try {
            fScript.Load();
            return true;
            
        } catch (Exception ex) {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            EditorGUILayout.LabelField($"There was an error loading FScript {fScript.name}:", style);

            EditorGUILayout.HelpBox($"{ex.GetType().Name}: {ex.Message}", MessageType.Error);

            if (!(ex is FScriptException)) Debug.LogException(ex);
            return false;
        }
    }
}
}
