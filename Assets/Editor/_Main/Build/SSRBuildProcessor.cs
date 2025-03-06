using SuperSmashRhodes.Config.Global;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SuperSmashRhodes.Editor.Build {
public class SSRBuildProcessor : IPostprocessBuildWithReport {
    public int callbackOrder => 0;
    
    public void OnPostprocessBuild(BuildReport report) {
        if (report.summary.result != BuildResult.Cancelled) {
            OnSuccessfulBuild();
        }
    }

    private void OnSuccessfulBuild() {
        ApplicationGlobalSettings.inst.BumpBuildMetadata();
    }
}
}
