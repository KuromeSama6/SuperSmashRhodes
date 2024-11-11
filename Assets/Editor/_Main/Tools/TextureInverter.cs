using UnityEditor;
using UnityEngine;
using System.IO;

namespace SuperSmashRhodes.Editor {
public class TextureInverter : EditorWindow {
    private Texture2D originalTexture;

    [MenuItem("Tools/Texture Inverter")]
    public static void ShowWindow() {
        GetWindow<TextureInverter>("Texture Inverter");
    }

    private void OnGUI() {
        GUILayout.Label("Invert Texture Colors", EditorStyles.boldLabel);

        originalTexture = (Texture2D)EditorGUILayout.ObjectField("Texture to Invert", originalTexture, typeof(Texture2D), false);

        if (originalTexture != null && GUILayout.Button("Invert and Save")) {
            InvertAndSaveTexture(originalTexture);
        }
    }

    private void InvertAndSaveTexture(Texture2D texture) {
        // Convert the texture to a readable format if needed
        Texture2D readableTexture = GetReadableTexture(texture);

        // Invert each pixel's color
        for (int y = 0; y < readableTexture.height; y++) {
            for (int x = 0; x < readableTexture.width; x++) {
                Color color = readableTexture.GetPixel(x, y);
                color.r = 1.0f - color.r;
                color.g = 1.0f - color.g;
                color.b = 1.0f - color.b;
                readableTexture.SetPixel(x, y, color);
            }
        }

        readableTexture.Apply();

        // Save the inverted texture as a new PNG file
        string path = EditorUtility.SaveFilePanel("Save Inverted Texture", "", texture.name + "_inverted.png", "png");
        if (!string.IsNullOrEmpty(path)) {
            byte[] bytes = readableTexture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
            Debug.Log("Inverted texture saved to: " + path);
        } else {
            Debug.LogWarning("Save cancelled.");
        }

        // Clean up
        DestroyImmediate(readableTexture);
    }

    private Texture2D GetReadableTexture(Texture2D texture) {
        // Create a new texture with readable format
        RenderTexture tempRT = RenderTexture.GetTemporary(
            texture.width, texture.height, 0,
            RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

        Graphics.Blit(texture, tempRT);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tempRT;

        Texture2D readableTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        readableTexture.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
        readableTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tempRT);

        return readableTexture;
    }
}
}
