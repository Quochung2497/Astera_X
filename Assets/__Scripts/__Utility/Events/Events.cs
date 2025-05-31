namespace Course.Utility.Events
{
    /// <summary>
    /// Marker interface for defining event types in the application.
    /// </summary>
    public interface IEvent { }

    // Score Events

    /// <summary>
    /// Event representing the addition of score in the game.
    /// </summary>
    public struct AddScore : IEvent
    {
        /// <summary>
        /// The amount of score to be added.
        /// </summary>
        public float Amount { get; }
    
        /// <summary>
        /// Initializes a new instance of the <see cref="AddScore"/> struct.
        /// </summary>
        /// <param name="amount">The amount of score to add.</param>
        public AddScore(float amount)
        {
            Amount = amount;
        }
    }
  
    // GameState Events

    /// <summary>
    /// Event representing a change in the game state.
    /// </summary>
    public struct GameStateChangedEvent : IEvent
    {
        /// <summary>
        /// The new state of the game after the change.
        /// </summary>
        public GameState NewState { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameStateChangedEvent"/> struct.
        /// </summary>
        /// <param name="newState">The new game state.</param>
        public GameStateChangedEvent(GameState newState)
        {
            NewState = newState;
        }
    }
}