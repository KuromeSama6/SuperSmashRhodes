using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Battle;
using UnityEngine;

namespace SuperSmashRhodes.Runtime.Gauge {
public class Gauge_Exusiai_AmmoGauge : CharacterComponent {
    [Title("References")]
    public Transform muzzleSocket;
    public GameObject muzzleFlashPrefab;
    
    public List<Magazine> magazines { get; private set; } = new();
    public bool chambered { get; private set; }
    
    public Magazine currentMagazine => magazines.Count > 0 ? magazines[0] : null;
    public int displayCount => currentMagazine.ammo + (chambered ? 1 : 0);
    public bool mayFire => displayCount > 0;
    
    public override void OnRoundInit() {
        base.OnRoundInit();

        for (int i = 0; i < 2; i++) {
            magazines.Add(new Magazine(30));
        }
        chambered = true;
    }

    public bool Fire(bool playMuzzleFlash = true, bool consumeAmmo = true) {
        if (currentMagazine.ammo > 0) {
            if (consumeAmmo) {
                if (player.burst.driveRelease) {
                    player.burst.releaseFrames -= 8;
                    if (player.burst.releaseFrames <= 0) {
                        player.burst.EndDriveRelease();
                    }
                
                } else {
                    --currentMagazine.ammo;
                }   
                chambered = true;
            }
            
            if (playMuzzleFlash) PlayMuzzleFlash();
            return true;
        }
        if (chambered) {
            chambered = false;
            if (playMuzzleFlash) PlayMuzzleFlash();
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

    public void AddMagazine() {
        if (magazines.Count(c => c.ammo > 0) >= 5) return;
        magazines.Add(new Magazine(30));
    }

    private void Update() {
        // remove empty
        // if (magazines.Count > 5) {
        //     var firstEmpty = magazines.First(c => c.ammo == 0);
        //     magazines.Remove(firstEmpty);
        // }

        magazines.RemoveAll(c => c != currentMagazine && c.ammo == 0);
    }

    public void PlayMuzzleFlash() {
        var go = Instantiate(muzzleFlashPrefab, muzzleSocket.position, muzzleSocket.rotation, muzzleSocket);
        go.transform.localEulerAngles += new Vector3(0, 90, 0);
        go.transform.localScale = Vector3.one * 0.2f;
    }
    
    public class Magazine {
        public int ammo;
        
        public Magazine(int ammo) {
            this.ammo = ammo;
        }
    }
}
}
