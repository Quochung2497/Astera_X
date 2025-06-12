using System;
using System.Collections.Generic;
using Course.Attribute;
using Course.Core;
using Course.Level;
using Course.Utility.Events;
using UnityEngine;
using UnityEngine.Serialization;
using Utility.DependencyInjection;

namespace Course.UI
{
    public class CanvasInstaller : MonoBehaviour, IInstaller
    {
        [Header("GUI Components")]
        [SerializeField]
        private AttributeUI scoreUI;
        [SerializeField]
        private AttributeUI jumpUI;
        [SerializeField]
        private GameOverUI gameOverUI;
        [SerializeField]
        private ActionButton mainMenuButton;
        
        [Inject]
        private IScoreBehaviour _scoreBehaviour;
        [Inject]
        private IHealthBehaviour _healthBehaviour;

        [Inject] 
        private ILevel _level;

        public void AwakeInitialize()
        { }

        public void StartInitialize()
        {
            scoreUI.Initialize(_scoreBehaviour);
            jumpUI.Initialize(_healthBehaviour);
            gameOverUI.Initialize(_scoreBehaviour, _level);
            mainMenuButton.Initialize(() =>
            {
                EventBus<GameStateChangedEvent>.Raise(
                    new GameStateChangedEvent(GameState.preLevel)
                );
            });
        }

        private void Awake()
        {
            AwakeInitialize();
        }

        private void Start()
        {
            StartInitialize();
        }
    }
}
