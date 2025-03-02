using System;
using System.Collections.Generic;
using System.Text;
using LeTai.TrueShadow;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Global.LoadingScreen {
public class LoadingScreen : SingletonBehaviour<LoadingScreen> {
    [Title("References")]
    public CanvasGroup canvasGroup;
    public RectTransform spinner;
    public TMP_Text loadingText, loadingStatusText;
    public CanvasGroup cover;

    public bool visible { get; set; }
    public bool showCover { get; set; }
    public float coverFadeSpeed { get; set; } = 15f;
    
    private List<KeyValuePair<string, LoadingStatus>> loadingStatus = new();
    
    private void Start() {
        canvasGroup.alpha = 0;
    }

    private void Update() {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, visible ? 1 : 0, Time.deltaTime * 15f);
        cover.alpha = Mathf.Lerp(cover.alpha, showCover ? 1 : 0, Time.deltaTime * coverFadeSpeed);
        
        spinner.Rotate(new (0, 0, 1), 4f);
        if (!visible) return;

        {
            loadingText.text = string.Join("\n", loadingStatus.ConvertAll(p => p.Key));
            
            // loading status
            var sb = new StringBuilder();

            int index = 0;
            foreach (var pair in loadingStatus) {
                sb.Append(pair.Value switch {
                    LoadingStatus.WAITING => index == 0 && Time.time % 1f < .5f ? "检查中" : " ",
                    LoadingStatus.GOOD => "好",
                    LoadingStatus.BAD => "错误",
                    LoadingStatus.NA => "N/A",
                    _ => ""
                });
                
                if (pair.Value == LoadingStatus.WAITING) ++index;
                
                sb.Append("\n");
            }
            
            loadingStatusText.text = sb.ToString();
            loadingStatusText.GetComponent<TrueShadow>().CopyToTMPSubMeshes();
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(loadingText.transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(loadingStatusText.transform as RectTransform);
        }
        
    }

    public void SetLoadingChecklist(params string[] items) {
        loadingStatus.Clear();
        coverFadeSpeed = 15f;
        foreach (var item in items) {
            loadingStatus.Add(new(item, LoadingStatus.WAITING));
        }
    }

    public void UpdateLoadingStatus(LoadingStatus status) {
        var index = loadingStatus.FindIndex(p => p.Value == LoadingStatus.WAITING);
        if (index == -1) return;
        loadingStatus[index] = new(loadingStatus[index].Key, status);
    }
}

public enum LoadingStatus {
    WAITING,
    GOOD,
    BAD,
    NA
}
}
