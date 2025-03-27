using System.IO;
using Newtonsoft.Json;
using SuperSmashRhodes.Framework;
using UnityEngine;

namespace SuperSmashRhodes.Settings {
public class SettingsManager : GlobalSingleton<SettingsManager> {
    public static string settingsPath => Path.Join(Application.persistentDataPath, "/local/settings.json");
    public SettingsObject data { get; private set; }
    
    public SettingsManager() {
        var path = settingsPath;
        if (File.Exists(path)) {
            data = JsonConvert.DeserializeObject<SettingsObject>(File.ReadAllText(path));
            
        } else {
            data = new();
            Save();
        }
    }

    public void Save() {
        // File.Create(settingsPath).Close();
        File.WriteAllText(settingsPath, JsonConvert.SerializeObject(data));
    }
}
}
