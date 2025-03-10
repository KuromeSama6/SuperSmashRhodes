using System;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Framework;
using SuperSmashRhodes.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuperSmashRhodes.UI.Global {
public class DebugLogsUI : SingletonBehaviour<DebugLogsUI> {
    [Title("References")]
    public GameObject logEntryPrefab;
    public RectTransform container;
    public Button clearLogsButton;
    public CanvasGroup canvasGroup;
    public GameObject controlIndicator;
    
    public bool visible { get; set; }
    private bool hasControl = false;
    
    private readonly Queue<LogEntry> logsQueue = new();
    
    private void Start() {
        Application.logMessageReceivedThreaded += HandleLogUnsafe;
        visible = false;
        canvasGroup.alpha = 0;
        
        clearLogsButton.onClick.AddListener(() => {
            container.gameObject.ClearChildren();
        });
    }

    private void Update() {
        lock (logsQueue) {
            while (logsQueue.Count > 0) {
                var entry = logsQueue.Dequeue();
                var go = Instantiate(logEntryPrefab, container);
                go.transform.SetAsFirstSibling();

                var text = go.GetComponent<TMP_Text>();
                var sb = new StringBuilder();

                sb.AppendLine($"[{entry.type}] [{DateTime.Now:HH:mm:ss} F{Time.frameCount}] {entry.str}");
                
                if (entry.type == LogType.Exception && !string.IsNullOrEmpty(entry.stackTrace)) {
                    sb.AppendLine(entry.stackTrace);
                }
                
                text.text = sb.ToString();
                text.color = entry.type switch {
                    LogType.Warning => Color.yellow,
                    LogType.Error => Color.red,
                    LogType.Exception => Color.red,
                    _ => Color.white
                };
                
                if (container.childCount > 1000) {
                    Destroy(container.GetChild(container.childCount - 1).gameObject);
                }
                
                LayoutRebuilder.ForceRebuildLayoutImmediate(container);
            }
        }
        
        // wave key
        if (UnityEngine.Input.GetKeyDown(KeyCode.BackQuote)) {
            visible = !visible;
        }

        if (visible) {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape)) {
                hasControl = !hasControl;
            }
        }
        
        controlIndicator.SetActive(!hasControl);
        canvasGroup.alpha = visible ? 1 : 0;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible && hasControl;
    }

    private void HandleLogUnsafe(string str, string stackTrace, LogType type) {
        if (Application.isEditor) return;
        
        lock (logsQueue) {
            logsQueue.Enqueue(new LogEntry {
                str = str, 
                stackTrace = stackTrace, 
                type = type
            });
        }
    }


    private struct LogEntry {
        public string str;
        public string stackTrace;
        public LogType type;
    }
}
}
