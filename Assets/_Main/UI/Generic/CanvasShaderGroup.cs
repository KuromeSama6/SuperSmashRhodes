using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI {
[RequireComponent(typeof(Canvas))]
[ExecuteInEditMode]
public class CanvasShaderGroup : MonoBehaviour {
    // The material with the color-inverting shader
    [SerializeField] private Material invertShaderMaterial;

    // A list to keep track of Graphic components and their original materials
    private readonly Dictionary<Graphic, Material> originalMaterials = new Dictionary<Graphic, Material>();

    private void Awake() {
        ApplyShaderToAllGraphics();
    }

    private void OnEnable() {
        ApplyShaderToAllGraphics();
    }

    private void OnDisable() {
        RestoreOriginalMaterials();
    }

    private void ApplyShaderToAllGraphics() {
        // Find all Graphic components in children
        Graphic[] graphics = GetComponentsInChildren<Graphic>();

        foreach (var graphic in graphics) {
            // Store the original material if we haven't done so already
            if (!originalMaterials.ContainsKey(graphic)) {
                originalMaterials[graphic] = graphic.material;
            }

            // Apply the invert shader material
            graphic.material = invertShaderMaterial;
        }
    }

    private void RestoreOriginalMaterials() {
        // Restore each graphic to its original material
        foreach (var entry in originalMaterials) {
            if (entry.Key != null) {
                entry.Key.material = entry.Value;
            }
        }

        // Clear the dictionary to avoid stale references
        originalMaterials.Clear();
    }

    // Optionally, call this to reapply the shader to all graphics if they change
    [Button("Refresh")]
    public void Refresh() {
        RestoreOriginalMaterials();
        ApplyShaderToAllGraphics();
    }
}
}
