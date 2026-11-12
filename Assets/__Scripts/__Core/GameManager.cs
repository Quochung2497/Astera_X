using Course.Utility;
using Course.Utility.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility.DependencyInjection;

namespace Course.Core
{
    public class GameManager : PrivateSingleton<GameManager>
    {
        public GameState currentStates => _gameState.CurrentState;

        #region Private Fields

        [Inject]
        private GameStateModel _gameState;

        /// <summary>
        /// Event binding for handling game state changes.
        /// </summary>
        private EventBinding<GameStateChangedEvent> _onGameStateChanged;

        #endregion

        #region Public Methods

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        public void TogglePause() => RequestState(_gameState.GetPauseTarget());

        public void ToggleShop() => RequestState(_gameState.GetShopTarget());

        #endregion

        #region Unity API

        /// <summary>
        /// Unity's OnEnable method.
        /// Registers the event binding for game state changes.
        /// </summary>
        private void OnEnable()
        {
            _onGameStateChanged = new EventBinding<GameStateChangedEvent>(HandleStateChanged);
            EventBus<GameStateChangedEvent>.Register(_onGameStateChanged);
        }

        /// <summary>
        /// Unity's OnDisable method.
        /// Deregisters the event binding for game state changes.
        /// </summary>
        private void OnDisable()
        {
            EventBus<GameStateChangedEvent>.Deregister(_onGameStateChanged);
        }

        /// <summary>
        /// Unity's Awake method.
        /// Calls the base class's Awake method.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();           
        }

        private void Start()
        {
            EventBus<GameStateChangedEvent>.Raise(
                new GameStateChangedEvent(GameState.mainMenu));
        }

        #endregion

        #region Private Methods

        private void RequestState(GameState? target)
        {
            if (target.HasValue)
                EventBus<GameStateChangedEvent>.Raise(new GameStateChangedEvent(target.Value));
        }

        private void HandleStateChanged(GameStateChangedEvent change)
        {
            _gameState.SetState(change.NewState);
            Time.timeScale = _gameState.ShouldFreezeSimulation ? 0f : 1f;
        }

        #endregion
    }
}


