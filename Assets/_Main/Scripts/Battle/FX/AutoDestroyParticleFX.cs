using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuperSmashRhodes.Battle.FX {
public class AutoDestroyParticleFX : MonoBehaviour {
    private List<ParticleSystem> systems = new();

    private void Start() {
        systems.AddRange(GetComponentsInChildren<ParticleSystem>());
    }

    private void Update() {
        systems.RemoveAll(c => c.isStopped);
        if (systems.Count == 0) {
            Destroy(gameObject);
        }
    }
}
}
