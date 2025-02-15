using System.Collections;

namespace SuperSmashRhodes.Battle {
/**
 * Represents a move that can be charged.
 */
public interface IChargable {
    int chargeEntryFrame { get; }
    bool mayCharge { get; }
    IEnumerator ChargeRoutine();
}
}
