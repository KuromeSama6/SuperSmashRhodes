namespace SuperSmashRhodes.Battle.Game {
/// <summary>
/// Represents a component that can be manually updated by the GameStateManager.
/// </summary>
public interface IEngineUpdateListener {
    /// <summary>
    /// Nice value for sorting. Lower values are updated first.
    /// </summary>
    int niceness => 0;
    
    /// <summary>
    /// Called once per game frame, in Update().
    /// This is not managed by the GameStateManager.
    /// Use this for cosemetic updates.
    /// </summary>
    void ManualUpdate() {
        
    }
    
    /// <summary>
    /// Called once per logical frame, in FixedUpdate(). This is called before inputs are solidified and sent over the network.
    /// Use this for input processing.
    /// This is managed by the GameStateManager.
    /// </summary>
    void EnginePreUpdate() {
        
    }
    
    /// <summary>
    /// Called once per logical frame, in FixedUpdate().
    /// Use this for regular game logic.
    /// </summary>
    void EngineUpdate() {
        
    }
}
}
