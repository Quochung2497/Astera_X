using Course.Utility;
using Course.Utility.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Course.Core
{
    public class GameManager : PrivateSingleton<GameManager>
    {
        #region Serialized Fields

        /// <summary>
        /// The current state of the game.
        /// Uses the <see cref="GameState"/> enum.
        /// </summary>
        [EnumFlag]
        public GameState currentStates { get; private set; } = GameState.none;

        #endregion

        #region Private Fields

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

        public void TogglePause()
        {
            GameState nextState;
            switch (currentStates)
            {
                case GameState.level:
                    nextState = GameState.paused;
                    break;
                case GameState.paused:
                    nextState = GameState.level;
                    break;
                default:
                    return;
            }

            EventBus<GameStateChangedEvent>.Raise(new GameStateChangedEvent(nextState));
        }

        #endregion

        #region Unity API

        /// <summary>
        /// Unity's OnEnable method.
        /// Registers the event binding for game state changes.
        /// </summary>
        private void OnEnable()
        {
            _onGameStateChanged = new EventBinding<GameStateChangedEvent>(e =>
                SetState(e.NewState));
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

        /// <summary>
        /// Sets the current game state and performs actions based on the new state.
        /// </summary>
        /// <param name="newState">The new game state to set.</param>
        private void SetState(GameState newState)
        {
            currentStates = newState;
            switch (newState)
            {
                case GameState.mainMenu:
                case GameState.paused:
                case GameState.shop: 
                    Time.timeScale = 0f;
                    break;

                case GameState.level:
                case GameState.none:
                    Time.timeScale = 1f;
                    break;
                
                default:
                    Time.timeScale = 1f;
                    break;
            }
        }

        #endregion
    }
}


