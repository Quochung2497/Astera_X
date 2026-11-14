using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Course.Control;
using Course.ScriptableObject;
using Course.Services.Achievements;
using Course.Services.Shop;
using Course.Utility.Events;
using UnityEngine;
using Utility.DependencyInjection;

namespace Course.Services.SaveService
{
    public class SaveManager : MonoBehaviour, ISaveManager
    {
        private ISaveService _service;
        private EventBinding<GameStateChangedEvent> _gameOverBinding;
        // private EventBinding<GameStateChangedEvent> _preLevelBinding;
        
        [Inject]
        private ICustomization _customizationManager;
        [Inject]
        private IAchievementManager _achievementManager;
        [Inject]
        private IShopService _shopService;

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
        /// Finds all ISavable components in the scene and invokes Save(...) on each.
        /// </summary>
        private IEnumerator Save()
        {
            var savables = FindObjectsOfType<MonoBehaviour>()
                .OfType<ISavable>();
            foreach (var s in savables)
            {
                var task = s.Save(_service);
                if (!task.IsCompleted)
                    yield return new WaitUntil(() => task.IsCompleted);
                if (task.Exception != null)
                    Debug.LogError($"Save failed on {s.GetType().Name}: {task.Exception}");
            }
        }

        #endregion

        #region Unity API

        private void Awake()
        {
            _service = new SaveService();
        }

        private IEnumerator Start()
        {
            // Let component Start methods finish wiring their gameplay listeners first.
            yield return null;
            var savables = FindObjectsOfType<MonoBehaviour>()
                .OfType<ISavable>();
            foreach (var s in savables)
            {
                var task = s.Load(_service);
                if (!task.IsCompleted)
                    yield return new WaitUntil(() => task.IsCompleted);
                if (task.Exception != null)
                    Debug.LogError($"Load failed on {s.GetType().Name}: {task.Exception}");
            }

            // Restore replays OnUnlocked. Subscribe to autosave only after loading all data.
            _customizationManager.OnCustomizationChanged += HandleSave;
            _achievementManager.OnUnlocked += HandleSaveOnUnlock;
            _achievementManager.OnHighScore += HandleHighScoreSave;
            _shopService.OnShopUpdate += HandleSave;
            _gameOverBinding = new EventBinding<GameStateChangedEvent>(OnGameStateChanged);
            EventBus<GameStateChangedEvent>.Register(_gameOverBinding);
        }

        private void OnDestroy()
        {
            EventBus<GameStateChangedEvent>.Deregister(_gameOverBinding);

            if (_shopService != null)
                _shopService.OnShopUpdate -= HandleSave;

            if (_customizationManager != null)
                _customizationManager.OnCustomizationChanged -= HandleSave;

            if (_achievementManager != null)
            {
                _achievementManager.OnUnlocked -= HandleSaveOnUnlock;
                _achievementManager.OnHighScore -= HandleHighScoreSave;
            }
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
