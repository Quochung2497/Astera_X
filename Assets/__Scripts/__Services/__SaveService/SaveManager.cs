using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Course.Control;
using Course.ScriptableObject;
using Course.Services.Achievements;
using Course.Utility;
using Course.Utility.Events;
using UnityEngine;
using Utility.DependencyInjection;

namespace Course.Services.SaveService
{
    public class SaveManager : PrivateSingleton<SaveManager>, ISaveManager
    {
        private ISaveService _service;
        private EventBinding<GameStateChangedEvent> _gameOverBinding;
        private Action _onCustomizationLoaded;
        // private EventBinding<GameStateChangedEvent> _preLevelBinding;
        
        [Inject]
        private ICustomization _customizationManager;
        [Inject]
        private IAchievementManager _achievementManager;

        #region ISaveManager Implementation

        /// <summary> Async save </summary>
        public Task SaveData<T>(string key, T value, ISavable self)
            => _service.SaveData(key, value, self);

        /// <summary> Async load </summary>
        public Task<T> LoadData<T>(string key, T defaultValue, ISavable self)
            => _service.LoadData(key, defaultValue, self);

        /// <summary> Remove one key </summary>
        public void DeleteData(string key)
            => _service.DeleteData(key);

        /// <summary> Wipe everything </summary>
        public void DeleteAll()
        {
#if UNITY_EDITOR
            _service.DeleteAll();
            Debug.Log("[SaveManager] All data wiped (Editor only).");
#endif
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds all ISavable components in the scene and invokes Load(...) on each.
        /// </summary>
        private IEnumerator Save()
        {
            var savables = FindObjectsOfType<MonoBehaviour>()
                .OfType<ISavable>();
            foreach (var s in savables)
            {
                var task = s.Save(_service);
                // wait until that Task completes
                yield return new WaitUntil(() => task.IsCompleted);
                if (task.Exception != null)
                    Debug.LogError($"Save failed on {s.GetType().Name}: {task.Exception}");
            }
        }

        private IEnumerator Load()
        {
            // Let component Start methods finish wiring their gameplay listeners first.
            yield return null;
            var savables = FindObjectsOfType<MonoBehaviour>()
                .OfType<ISavable>();
            foreach (var s in savables)
            {
                var task = s.Load(_service);
                yield return new WaitUntil(() => task.IsCompleted);
                if (task.Exception != null)
                    Debug.LogError($"Load failed on {s.GetType().Name}: {task.Exception}");
            }
        }

        #endregion

        #region Unity API

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            _service = new SaveService();
        }

        private void Start()
        {
            _onCustomizationLoaded = () => {
                _customizationManager.OnCustomizationChanged += HandleSave;
            };
            _customizationManager.OnLoaded += _onCustomizationLoaded;
            _achievementManager.OnUnlocked += HandleSaveOnUnlock;
            _achievementManager.OnHighScore += HandleHighScoreSave;
            _gameOverBinding = new EventBinding<GameStateChangedEvent>(OnGameStateChanged);
            EventBus<GameStateChangedEvent>.Register(_gameOverBinding);
            StartCoroutine(Load());
        }

        private void OnDestroy()
        {
            if(_achievementManager == null || _customizationManager == null)
                return;
            _customizationManager.OnCustomizationChanged -= HandleSave;
            _customizationManager.OnLoaded      -= _onCustomizationLoaded;
            _achievementManager.OnUnlocked -= HandleSaveOnUnlock;
            _achievementManager.OnHighScore -= HandleHighScoreSave;
            EventBus<GameStateChangedEvent>.Deregister(_gameOverBinding);
        }
        
        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            if (evt.NewState == GameState.gameOver || evt.NewState == GameState.preLevel)
            {
                // best-effort save right when we enter Game Over
                StartCoroutine(Save());
            }
        }
        
        private void HandleSave()
        {
            StartCoroutine(Save());
        }

        // Called when an AchievementSO is unlocked
        private void HandleSaveOnUnlock(AchievementSO def)
        {
            // you have the `def` here if you ever need it
            StartCoroutine(Save());
        }

        // Called when a new high score fires (string title, float score)
        private void HandleHighScoreSave(string title, float score)
        {
            StartCoroutine(Save());
        }

        // private void OnApplicationQuit()
        // {
        //     // fire-and-forget: best effort to save before shutdown
        //     StartCoroutine(Save());
        // }
        //
        // private void OnApplicationPause(bool paused)
        // {
        //     if (paused)
        //     {
        //         // on mobile background, save progress
        //         StartCoroutine(Save());
        //     }
        // }

        // private void Update()
        // {
        //     if(UnityEngine.Input.GetKeyDown(KeyCode.Delete))
        //         DeleteAll();
        //     if(UnityEngine.Input.GetKeyDown(KeyCode.Space))
        //         StartCoroutine(Save());
        // }

        #endregion
    }
}
