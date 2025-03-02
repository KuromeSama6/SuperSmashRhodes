using System;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Framework;
using UnityEngine;

namespace SuperSmashRhodes.Config.Global {
public class ApplicationGlobalSettings : GlobalDataObject<ApplicationGlobalSettings> {
    [Title("Versioning")]
    public string branch;
    public string semver;
    public ushort rollbitVersion;
    
    [Title("Build Metadata")]
    [ReadOnly]
    public long buildNumber;
    [ReadOnly]
    public long buildTime;
    
    public void BumpBuildMetadata() {
        buildNumber++;
        buildTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        SaveNow();
    }
}
}
