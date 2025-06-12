using System;
using System.Collections;
using Course.Core;
using Course.Utility.Events;
using UnityEngine;

namespace Course.Level
{
    public class LevelBehaviour : MonoBehaviour, ILevel
    {
        public event Action OnPreLevel;   
        // Event: fired whenever a new level starts. Subscribers receive the new level number.
        public event Action OnLevelStarted;

        // Event: fired when the current level is cleared (before incrementing).
        public event Action OnLevelCleared;

        // Expose the current level index as a read-only property.
        public int CurrentLevel { get; private set; }

        /// <summary>
        /// Call this once at the very beginning (e.g. in a “GameInstaller” or Awake of some Bootstrap
        /// script) to set the starting level. It will immediately fire OnLevelStarted(startLevel).
        /// </summary>
        public void Initialize(int startLevel)
        {
            CurrentLevel = startLevel;
        }

        /// <summary>
        /// Call this whenever the player has cleared all asteroids in the current level.
        /// It first fires OnLevelCleared(currentLevel), then increments and fires OnLevelStarted(newLevel).
        /// </summary>
        public void LevelCleared()
        {
            // 1) Notify listeners that level CurrentLevel has just been cleared
            OnLevelCleared?.Invoke();

            // 2) Bump to the next level
            CurrentLevel++;

            // 3) Notify listeners that level CurrentLevel has started
            RaisePreLevel();
        }
        
        private void RaisePreLevel()
        {
            EventBus<GameStateChangedEvent>.Raise(
                new GameStateChangedEvent(GameState.preLevel)
            );
            OnPreLevel?.Invoke();
        }
        
        public void StartNextLevel()
        {
            if(GameManager.TryGetInstance().currentStates != GameState.level)
                EventBus<GameStateChangedEvent>.Raise(new GameStateChangedEvent(GameState.level));
            EventBus<LevelReached>.Raise(new LevelReached(CurrentLevel));
            OnLevelStarted?.Invoke();
        }
    }
}


