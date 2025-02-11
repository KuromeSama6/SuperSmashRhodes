using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperSmashRhodes.Battle.FX {
public class ParticleFXPlayer : MonoBehaviour {
    private List<ParticleSystem> systems = new();

    public bool allDone => systems.Count == 0;
    
    private void Start() {
        systems.AddRange(GetComponentsInChildren<ParticleSystem>());
    }

    private void Update() {
        systems.RemoveAll(c => c.isStopped);
        if (systems.Count == 0) {
            Destroy(gameObject);
        }
    }

    public IEnumerator WaitUntilComplete() {
        yield return new WaitUntil(() => systems.Count == 0);
    }
}
}
