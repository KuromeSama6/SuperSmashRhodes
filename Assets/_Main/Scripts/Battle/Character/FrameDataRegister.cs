using UnityEngine;

namespace SuperSmashRhodes.Battle {
public class FrameDataRegister : RuntimeCharacterDataRegister {
    public int hitstunFrames { get => _hitstunFrames + carriedHitstunFrames; }
    public int blockstunFrames { get; set; }

    private int _hitstunFrames;
    private int carriedHitstunFrames;
    
    public FrameDataRegister(PlayerCharacter owner) : base(owner) { }

    public void Tick() {
        if (carriedHitstunFrames > 0) {
            --carriedHitstunFrames;
        } else if (_hitstunFrames > 0) {
            --_hitstunFrames;
        }
        
        if (blockstunFrames > 0) {
            --blockstunFrames;
        }
    }

    public void SetHitstunFrames(int frames, int carried) {
        _hitstunFrames = frames;
        
        // Carried hitstun frames are capped
        if (owner.comboCounter.count > 0) {
            // carriedHitstunFrames = Mathf.Max(carriedHitstunFrames + carried, 8);
        }
    }
    
}
}
