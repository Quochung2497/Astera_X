using System;
using Course.Core;
using Course.Utility.Events;
using UnityEngine;
using Utility.DependencyInjection;

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

        public SpawnRule? CurrentSpawnRule { get; private set; }

        [Inject]
        private ILevelSpawnRules _spawnRules;

        public void Initialize(int startLevel)
        {
            CurrentLevel = startLevel;
            CurrentSpawnRule = null;
        }

        public void LevelCleared()
        {
            OnLevelCleared?.Invoke();
            CurrentLevel++;
            EnterPreLevel();
        }

        public void EnterPreLevel()
        {
            CurrentSpawnRule = _spawnRules.PrepareLevel(CurrentLevel);

            OnPreLevel?.Invoke();
            EventBus<GameStateChangedEvent>.Raise(new GameStateChangedEvent(GameState.preLevel));
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


