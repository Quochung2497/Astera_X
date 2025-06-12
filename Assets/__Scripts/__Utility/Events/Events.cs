using System;
using Course.Attribute.Bullet;
using Course.ScriptableObject;

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
    
        // 1) FIRST DUST – Shot Your First Asteroid
    // Fired any time the player shoots an asteroid.
    public struct AsteroidShot : IEvent
    {
        // (no extra data needed)
    }


    // 2) LUCKY SHOT – Bullet Wrapped Screen
    // Fired whenever a bullet wraps around the screen.
    public struct BulletWrapped : IEvent
    {
        /// <summary>
        /// Unique identifier for this bullet instance
        /// </summary>
        public readonly Guid BulletId;
        public BulletWrapped(Guid guid) => BulletId = guid;
    }


    // 2b) LUCKY SHOT – Hit an Asteroid after wrapping
    // Tell us which bullet hit the asteroid.
    public struct AsteroidHitByBullet : IEvent
    {
        public readonly Guid BulletId;
        public AsteroidHitByBullet(Guid guid) => BulletId = guid;
    }


    // 3) TRIGGER HAPPY – 1,000 Shots Fired
    // Fired every time the player fires a shot.
    public struct ShotFired : IEvent
    {
        // no payload; you’ll just count occurrences
    }


    // 4) ROOKIE PILOT – Score Above 10,000
    // You already have AddScore, which carries the delta.
    // Here’s an alternate “total‐score” event if you prefer:
    public struct ScoreUpdated : IEvent
    {
        /// <summary>
        /// The player’s total score after this change.
        /// </summary>
        public float TotalScore { get; }

        public ScoreUpdated(float totalScore)
        {
            TotalScore = totalScore;
        }
    }


    // 6) SKILLFUL DODGER – Reach Level 5
    // Fired whenever the player enters a new level.
    public struct LevelReached : IEvent
    {
        public int LevelNumber { get; }
        public LevelReached(int levelNumber)
        {
            LevelNumber = levelNumber;
        }
    }
    
    // EAGLE EYE
    public struct LuckyShotOccurred : IEvent { }

}