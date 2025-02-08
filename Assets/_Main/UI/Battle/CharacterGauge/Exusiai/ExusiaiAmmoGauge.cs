using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Runtime.Gauge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Battle.Exusiai {
public class ExusiaiAmmoGauge : ComponentSpecificUIElement<Gauge_Exusiai_AmmoGauge> {
    [Title("References")]
    public Image chamberedIndicator;
    public TMP_Text ammoText;
    public RectTransform magazineContainer;
    public GameObject magazinePrefab;

    private List<MagazineIndicator> magIndicators { get; } = new();
    
    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
        if (!playerComponent || !player) return;

        if (magIndicators.Count != playerComponent.magazines.Count || magIndicators[0].magazine != playerComponent.currentMagazine) {
            RefreshMagIndicators();
        }
        
        chamberedIndicator.gameObject.SetActive(playerComponent.chambered);
        var current = playerComponent.currentMagazine;
        ammoText.text = $"{current.ammo.ToString().PadLeft(2, '0')}";
    }

    private void RefreshMagIndicators() {
        foreach (var mag in magIndicators) {
            Destroy(mag.gameObject);
        }
        magIndicators.Clear();
        
        // create
        foreach (var mag in playerComponent.magazines) {
            var magObj = Instantiate(magazinePrefab, magazineContainer);
            var magInd = magObj.GetComponent<MagazineIndicator>();
            magInd.gauge = playerComponent;
            magInd.magazine = mag;
            magIndicators.Add(magInd);
        }
    }
    
}
}
