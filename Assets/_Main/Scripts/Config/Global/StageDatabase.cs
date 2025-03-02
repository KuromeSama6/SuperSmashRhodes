using System.Collections.Generic;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.Stage;
using SuperSmashRhodes.Framework;

namespace SuperSmashRhodes.Config.Global {
public class StageDatabase : GlobalDataObject<StageDatabase> {
    [Title("Stages")]
    public List<StageData> stages = new();
    public List<StageBGMData> bgms = new();
}
}
