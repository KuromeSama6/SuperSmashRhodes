using System.Collections.Generic;

namespace SuperSmashRhodes.Battle.State {
public class EntityStateData {
    // Cancel options
    /// <summary>
    /// States that can be canceled into from this state.
    /// </summary>
    public List<EntityState> cancelOptions { get; } = new List<EntityState>();

    /// <summary>
    /// A flag that determines if the current state can be cancelled into a specific type of states.
    /// </summary>
    public EntityStateType cancelFlag;

    public bool disableSideSwap = false;
}
}
