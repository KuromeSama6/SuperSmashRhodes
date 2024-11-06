using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Instruction;
using UnityEngine;

namespace SuperSmashRhodes.FScript {
public class FScriptMainProcedure : FScriptProcedure {
    public FrameDataOverview overview { get; private set; }
    
    public FScriptMainProcedure(FBlock block) : base(block) {
        // create overview
        overview = new FrameDataOverview() {
            startup = CountFramesAfter<MoveStartupInstruction>(),
            active = CountFramesAfter<MoveActiveInstruction>(),
            recovery = CountFramesAfter<MoveRecoveryInstruction>(),
            total = totalFrames
        };
    }
}

public struct FrameDataOverview {
    public int startup, active, recovery, total;
    public int onHit, onBlock;
}
}
