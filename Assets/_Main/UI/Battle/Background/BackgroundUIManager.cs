using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Battle {
public class BackgroundUIManager : SingletonBehaviour<BackgroundUIManager> {
    [Title("References")]
    public Image backgroundDim;
    public GameObject backgroundContainer;
    public UDictionary<BackgroundType, GameObject> backgroundPrefabs;

    public float backgroundAlpha {
        get => backgroundDim.color.a;
        set => backgroundDim.color = backgroundDim.color.ApplyAlpha(value);
    }

    public BackgroundUIData data {
        get {
            var p1 = GameManager.inst.GetPlayer(0);
            var p2 = GameManager.inst.GetPlayer(1);
            if (!p1 || !p2) return BackgroundUIData.DEFAULT;
            return p2.activeState.stateData.backgroundUIData.priority > p1.activeState.stateData.backgroundUIData.priority ? p2.activeState.stateData.backgroundUIData : p1.activeState.stateData.backgroundUIData;
        }
    }
    
    private void Start() {
        Clear();
    }

    private void Update() {
        var backgroundData = data;
        backgroundAlpha = Mathf.Lerp(backgroundAlpha, backgroundData.dimAlpha, Time.deltaTime * backgroundData.dimSpeed);

        if (backgroundData.bgType != BackgroundType.NONE) {
            backgroundContainer.SetActive(true);
            backgroundPrefabs.Values.ForEach(c => c.SetActive(false));
            var comp = backgroundPrefabs[backgroundData.bgType].GetComponent<Image>();
            comp.color = backgroundData.bgColor;
        } else {
            backgroundContainer.SetActive(false);
        }
    }

    public void Clear() {
        backgroundContainer.SetActive(false);
        backgroundPrefabs.Values.ForEach(c => c.SetActive(false));
    }

}

public enum BackgroundType {
    NONE,
    SUPER,
    BURST
}

public struct BackgroundUIData {
    public int priority;
    public float dimAlpha;
    public float dimSpeed;
    public BackgroundType bgType;
    public Color bgColor;
    
    public BackgroundUIData(int priority, float dimAlpha, float dimSpeed, BackgroundType bgType, Color bgColor) {
        this.priority = priority;
        this.dimAlpha = dimAlpha;
        this.dimSpeed = dimSpeed;
        this.bgType = bgType;
        this.bgColor = bgColor;
    }
    
    public static BackgroundUIData DEFAULT => new(0, 0, 1, BackgroundType.NONE, Color.white);
}
}
