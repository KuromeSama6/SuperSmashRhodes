using System.Collections.Generic;
using SuperSmashRhodes.Battle;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.Gauge {
public class Gauge_Exusiai_AmmoGauge : CharacterComponent {
    public List<Magazine> magazines { get; private set; } = new();
    public bool chambered { get; private set; }
    
    public Magazine currentMagazine => magazines.Count > 0 ? magazines[0] : null;
    public int displayCount => currentMagazine.ammo + (chambered ? 1 : 0);
    public bool mayFire => displayCount > 0;
    
    public override void OnRoundInit() {
        base.OnRoundInit();

        for (int i = 0; i < 4; i++) {
            magazines.Add(new Magazine(30));
        }
        chambered = true;
    }

    public bool Fire() {
        if (currentMagazine.ammo > 0) {
            --currentMagazine.ammo;
            chambered = true;
            return true;
        }
        
        if (chambered) {
            chambered = false;
            return true;
        }
        
        return false;
    }

    public void Reload() {
        if (magazines.Count < 2) return;
        var mag = currentMagazine;
        magazines.RemoveAt(0);
        magazines.Add(mag);
        
        if (!chambered && currentMagazine.ammo > 0) {
            chambered = true;
            currentMagazine.ammo--;
        }
    }

    public class Magazine {
        public int ammo;
        
        public Magazine(int ammo) {
            this.ammo = ammo;
        }
    }
}
}
