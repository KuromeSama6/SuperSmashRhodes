using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace SuperSmashRhodes.Util {
public class AppDaemon : PersistentSingletonBehaviour<AppDaemon> {
    public static UnityEvent onApplicationQuit { get; } = new();
    public static UnityEvent<bool> onApplicationPause { get; } = new();
    public static UnityEvent<bool> onApplicationFocus { get; } = new();

    private static string logFilePath => Path.Combine(Application.persistentDataPath, "log.txt");

    private void OnApplicationQuit() {
        onApplicationQuit.Invoke();
    }

    private void OnApplicationPause(bool pauseStatus) {
        onApplicationPause.Invoke(pauseStatus);
    }

    private void OnApplicationFocus(bool hasFocus) {
        onApplicationFocus.Invoke(hasFocus);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init() {
        var go = new GameObject("ApplicationStatusHelper");
        go.AddComponent<AppDaemon>();

        //TODO Frame rate in settings
        Application.targetFrameRate = Mathf.Max(120, Application.targetFrameRate);
        
        File.WriteAllText(logFilePath, string.Empty);
        Application.logMessageReceived += HandleLog;
        
    }

    private static void HandleLog(string logString, string stackTrace, LogType type) {
        // Combine the log message, stack trace, and log type
        string logEntry = $"{System.DateTime.Now}: [{type}] {logString}\n";
        if (type == LogType.Exception) {
            logEntry += $"StackTrace: {stackTrace}\n";
        }

        // Write the log entry to the file
        File.AppendAllText(logFilePath, logEntry);
    }
}
}
