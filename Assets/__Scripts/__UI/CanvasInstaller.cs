using Course.Attribute;
using Course.Control;
using Course.Core;
using Course.Level;
using Course.Services.Achievements;
using Course.Utility.Events;
using UnityEngine;
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
        private IAchievementManager _achievementManager;
        [Inject] 
        private ICustomization _manager;
        [Inject] 
        private ILevel _level;

        public void AwakeInitialize()
        { }

        public void StartInitialize()
        {
            scoreUI.Initialize(_scoreBehaviour,_manager);
            jumpUI.Initialize(_healthBehaviour,_manager);
            gameOverUI.Initialize(
                _scoreBehaviour, _level, _achievementManager,
                GameManager.TryGetInstance().RestartGame);
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
