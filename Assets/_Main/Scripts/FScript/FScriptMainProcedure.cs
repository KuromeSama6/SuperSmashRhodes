using SuperSmashRhodes.FScript.Components;
using SuperSmashRhodes.FScript.Instruction;
using SuperSmashRhodes.FScript.Util;
using UnityEngine;

namespace SuperSmashRhodes.FScript {
public class FScriptMainProcedure : FScriptProcedure {
    public FrameDataOverview overview { get; private set; }
    
    public FScriptMainProcedure(FBlock block, AddressRegistry registry) : base(block, registry) {
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
