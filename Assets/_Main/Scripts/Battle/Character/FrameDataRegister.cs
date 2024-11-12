using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class FrameDataRegister : RuntimeCharacterDataRegister {
    public int hitstunFrames { get; set; }
    public int blockstunFrames { get; set; }
    
    public FrameDataRegister(PlayerCharacter owner) : base(owner) { }

    public void Tick() {
        if (hitstunFrames > 0) --hitstunFrames;
        if (blockstunFrames > 0) {
            --blockstunFrames;
        }
    }
    
}
}
