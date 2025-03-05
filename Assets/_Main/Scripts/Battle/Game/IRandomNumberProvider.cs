using System;

namespace SuperSmashRhodes.Battle.Game {
public interface IRandomNumberProvider {
    double Next();

    int Range(int minInclusive, int maxExclusive) {
        var seed = Next();
        return minInclusive + (int)(seed * (maxExclusive - minInclusive));
    }

    float Range(float minInclusive, float maxExclusive) {
        var seed = Next();
        return (float)(minInclusive + seed * (maxExclusive - minInclusive));
    }

    double Range(double minInclusive, double maxExclusive) {
        var seed = Next();
        return minInclusive + seed * (maxExclusive - minInclusive);
    }

    bool Chance(float probability = 0.5f) {
        return Next() < probability;
    }

}

public class DefaultRandomNumberProvider : IRandomNumberProvider {
    private readonly Random random;

    public DefaultRandomNumberProvider() {
        random = new Random();
    }

    public double Next() {
        return random.NextDouble();
    }
}
}
