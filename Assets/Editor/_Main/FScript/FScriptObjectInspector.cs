using System;
using System.Collections.Generic;
using SuperSmashRhodes;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.FScript;
using SuperSmashRhodes.FScript.Instruction;
using UnityEditor;
using UnityEngine;

namespace FScript.Editor {
[CustomEditor(typeof(FScriptObject))]
public class FScriptObjectInspector : UnityEditor.Editor {
    private FScriptLinked scriptLinked;
    private bool loaded = false;
    private bool inspectorShowAddressTable = true;
    private DateTime loadTime;
    
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
        
        EditorGUILayout.LabelField($"Loaded as of {loadTime}");

        if (scriptLinked != null) {
            // Address
            inspectorShowAddressTable = EditorGUILayout.Foldout(inspectorShowAddressTable, "Address Table");
            if (inspectorShowAddressTable) {
                EditorGUILayout.LabelField("ADDRESS -> NEXT", "INSTRUCTION");
                
                foreach (var entry in scriptLinked.addressRegistry.registry) {
                    if (entry.Value is SectionInstruction section)
                        EditorGUILayout.LabelField($"{entry.Key:X} -> {(section.nextAddress == 0 ? "[block end]" : section.nextAddress.ToString("X"))}", $"[section] {section.sectionName}", EditorStyles.boldLabel);
                    else if (entry.Value is FInstruction instruction)
                        EditorGUILayout.LabelField($"{entry.Key:X} -> {(instruction.nextAddress == 0 ? "[block end]" : instruction.nextAddress.ToString("X"))}", instruction.rawLine.raw);
                    else
                        EditorGUILayout.LabelField($"{entry.Key:X}", $"(unknown)");
                }
            }
        }

        EditorGUILayout.LabelField("Raw Content");
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextArea(fScript.rawScript);
        EditorGUI.EndDisabledGroup();
    }

    private bool Load(FScriptObject fScript) {
        try {
            string[] guids = AssetDatabase.FindAssets("t:FScriptLibrary");
            List<FScriptLibrary> scripts = new();
            foreach (var guid in guids) {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath<FScriptLibrary>(path);
                scripts.Add(obj);
            }

            scriptLinked = new FScriptLinked(fScript, scripts);
            loadTime = DateTime.Now;
            return true;

        } catch (Exception ex) {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            EditorGUILayout.LabelField($"There was an error loading FScript {fScript.name}:", style);

            EditorGUILayout.HelpBox($"{ex.GetType().Name}: {ex.Message}", MessageType.Error);

            Debug.LogException(ex);
            return false;
        }
    }
}
}
