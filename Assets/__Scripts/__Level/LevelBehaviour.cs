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

        public int CurrentLevel => _progression.CurrentLevel;

        public SpawnRule? CurrentSpawnRule => _progression.CurrentSpawnRule;

        [Inject]
        private ILevelProgression _progression;

        [Inject]
        private IGameStateReader _gameState;

        public void Initialize(int startLevel) => _progression.Initialize(startLevel);

        public void LevelCleared()
        {
            OnLevelCleared?.Invoke();
            _progression.AdvanceLevel();
            EnterPreLevel();
        }

        public void EnterPreLevel()
        {
            _progression.PrepareCurrentLevel();

            OnPreLevel?.Invoke();
            EventBus<GameStateChangedEvent>.Raise(new GameStateChangedEvent(GameState.preLevel));
        }
        
        public void StartNextLevel()
        {
            if (_gameState.CurrentState != GameState.level)
                EventBus<GameStateChangedEvent>.Raise(new GameStateChangedEvent(GameState.level));
            EventBus<LevelReached>.Raise(new LevelReached(CurrentLevel));
            OnLevelStarted?.Invoke();
        }
    }
}


