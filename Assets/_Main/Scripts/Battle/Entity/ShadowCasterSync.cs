using Sirenix.OdinInspector;
using UnityEngine;

namespace SuperSmashRhodes.Character {
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ShadowCasterSync : MonoBehaviour {
    [Title("References")]
    public MeshFilter sourceMeshFilter;
    public MeshRenderer sourceRenderer;

    private MeshFilter targetMeshFilter;
    private MeshRenderer targetRenderer;

    private void Start() {
        targetMeshFilter = GetComponent<MeshFilter>();
        targetRenderer = GetComponent<MeshRenderer>();

        if (targetMeshFilter == null || targetRenderer == null) {
            Debug.LogError("Target object is missing MeshFilter or MeshRenderer!");
            enabled = false;
            return;
        }
    }

    private void Update() {
        if (sourceMeshFilter == null || sourceRenderer == null) return;

        // Sync Mesh
        if (targetMeshFilter.mesh != sourceMeshFilter.sharedMesh) {
            targetMeshFilter.mesh = sourceMeshFilter.sharedMesh;
        }

        // Sync Materials
        if (targetRenderer.sharedMaterials != sourceRenderer.sharedMaterials) {
            targetRenderer.sharedMaterials = sourceRenderer.sharedMaterials;
        }
    }
}
}
