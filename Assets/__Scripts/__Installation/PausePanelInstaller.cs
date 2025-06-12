using System;
using Course.Core;
using Course.UI;
using Course.Utility.Events;
using UnityEngine;

namespace Course.Installation
{
    public class PausePanelInstaller : MonoBehaviour , IInstaller
    {
        [SerializeField]
        private ActionButton pauseButton;

        private GameManager _gameManager;
        
        public void AwakeInitialize()
        { }

        public void StartInitialize()
        {
            _gameManager = GameManager.TryGetInstance();
            pauseButton?.Initialize(() =>
            {
                Debug.Log($"pause button intialize");
                if (_gameManager == null) return; 
                GameState newState = (_gameManager.currentStates == GameState.level)
                    ? GameState.paused
                    : GameState.level;
                EventBus<GameStateChangedEvent>.Raise(
                    new GameStateChangedEvent(newState)
                );
            });
        }

        private void Start()
        {
            StartInitialize();
        }
    }
}

