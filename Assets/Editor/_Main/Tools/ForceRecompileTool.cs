using UnityEditor;
using UnityEngine;

namespace SuperSmashRhodes.Editor {
public static class ForceRecompileTool {
    [MenuItem("Tools/Force Recompile")] // Shortcut: Ctrl+Shift+R (Cmd+Shift+R on Mac)
    private static void ForceRecompile() {
        // Trigger recompilation
        Debug.Log("Forced recompilation triggered.");
        AssetDatabase.Refresh();
        
    }
}

}
