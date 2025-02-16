using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.FX;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Battle {
public class BackgroundUIManager : SingletonBehaviour<BackgroundUIManager> {
    [Title("References")]
    public Canvas canvas;
    public Image backgroundDim;
    public GameObject backgroundContainer;
    public Image transitionMask;
    public GameObject flashPrefab;
    
    public UDictionary<BackgroundType, GameObject> backgroundPrefabs;
    public UDictionary<TransitionType, FlipbookData> transitions;

    private float flipbookTimer;
    
    public float backgroundAlpha {
        get => backgroundDim.color.a;
        set => backgroundDim.color = backgroundDim.color.ApplyAlpha(value);
    }

    public BackgroundUIData data {
        get {
            var p1 = GameManager.inst.GetPlayer(0);
            var p2 = GameManager.inst.GetPlayer(1);
            if (!p1 || !p2) return BackgroundUIData.DEFAULT;
            try {
                return p2.activeState.stateData.backgroundUIData.priority > p1.activeState.stateData.backgroundUIData.priority ? p2.activeState.stateData.backgroundUIData : p1.activeState.stateData.backgroundUIData;
            } catch {
                return BackgroundUIData.DEFAULT;
            }
        }
    }
    
    private void Start() {
        Clear();
    }

    private void Update() {
        var backgroundData = data;
        
        {
            var targetAlpha = backgroundData.dimAlpha;
            if (Mathf.Approximately(targetAlpha, 0)) {
                if (GameManager.inst.GetPlayer(0).burst.driveRelease || GameManager.inst.GetPlayer(1).burst.driveRelease) {
                    targetAlpha = 0.95f;
                }
            }
        
            backgroundAlpha = Mathf.Lerp(backgroundAlpha, targetAlpha, Time.deltaTime * backgroundData.dimSpeed);   
        }

        if (backgroundData.bgType != BackgroundType.NONE) {
            backgroundContainer.SetActive(true);
            
            backgroundPrefabs.Keys.ForEach(c => backgroundPrefabs[c].SetActive(c == backgroundData.bgType));
            
            var comp = backgroundPrefabs[backgroundData.bgType].GetComponent<Image>();
            comp.color = backgroundData.bgColor;
            
            // transition mask
            if (backgroundData.transition != TransitionType.NONE) {
                transitionMask.enabled = true;
                var flipbook = transitions[backgroundData.transition];

                if (backgroundData.transitionFrame < Math.Min(flipbook.frames, flipbook.sprites.Count)) { 
                    if (flipbookTimer == 0) {
                        transitionMask.sprite = flipbook.sprites[backgroundData.transitionFrame];
                        ++backgroundData.transitionFrame;
                        flipbookTimer += Time.deltaTime;

                    } else {
                        flipbookTimer += Time.deltaTime;
                        if (flipbookTimer >= 1f / flipbook.frameRate) {
                            flipbookTimer = 0;
                        }
                        
                    }
                    
                } else {
                    transitionMask.sprite = null;
                }

            } else {
                transitionMask.enabled = false;
            }
            
        } else {
            backgroundContainer.SetActive(false);
        }
    }

    public void Clear() {
        backgroundContainer.SetActive(false);
        backgroundPrefabs.Values.ForEach(c => c.SetActive(false));
    }

    public void Flash(float duration) {
        var go = Instantiate(flashPrefab, canvas.transform);
        this.CallLaterCoroutine(duration, () => Destroy(go));
    }

}

public enum BackgroundType {
    NONE,
    SUPER,
    BURST
}

public enum TransitionType {
    NONE,
    SUPER_FADE_IN
}

public class BackgroundUIData {
    public int priority;
    public float dimAlpha;
    public float dimSpeed;
    public BackgroundType bgType;
    public Color bgColor;
    public TransitionType transition;
    public int transitionFrame;
    
    public BackgroundUIData(
        int priority = 0, 
        float dimAlpha = 0, 
        float dimSpeed = 1, 
        BackgroundType bgType = BackgroundType.NONE, 
        Color? bgColor = null, 
        TransitionType transition = TransitionType.NONE,
        int transitionFrame = 0
        ) {
        
        this.priority = priority;
        this.dimAlpha = dimAlpha;
        this.dimSpeed = dimSpeed;
        this.bgType = bgType;
        this.bgColor = bgColor ?? Color.white;
        this.transition = transition;
        this.transitionFrame = transitionFrame;
    }

    public static BackgroundUIData DEFAULT => new();
}
}
