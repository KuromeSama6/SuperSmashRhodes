using SuperSmashRhodes.Battle.Game;
using UnityEngine;

namespace SuperSmashRhodes.Match {
public class PredeterminedRandom : IRandomNumberProvider {
    private readonly double[] seeds;
    private int pointer;
    
    public PredeterminedRandom(params double[] seeds) {
        this.seeds = seeds;
        pointer = 0;
    }

    public double Next() {
        if (pointer >= seeds.Length) {
            pointer = 0;
            Debug.LogWarning($"PredeterminedRandom seed list ({seeds.Length}) exhausted, starting over. You may need to increase the seed list size.");
        }
        
        return seeds[pointer++];
    }
}
}
