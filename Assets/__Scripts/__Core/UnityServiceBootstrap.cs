using System;
using System.Collections.Generic;
using Course.Control;
using Course.ScriptableObject;
using Course.ScriptableObject.UnityServices;
using Course.Services.Achievements;
using Course.Services.Unity;
using Course.Utility;
using Course.Utility.Events;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility.DependencyInjection;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;

namespace Course.Core
{
    public class UnityServiceBootstrap : PrivateSingleton<UnityServiceBootstrap>
    {
        private int   _currentLevel;
        private int   _lastScore;
        private bool  _didHitHighThisSession;
        private Dictionary<PartType, int> _partSelectionCounts = new();

        private EventBinding<LevelReached>         _levelEvent;
        private EventBinding<ScoreUpdated>         _scoreEvent;
        private EventBinding<GameStateChangedEvent> _gameOverEvent;
        private IAdsService _adsService;

        [SerializeField] private AsteroidsConfig asteroidsConfig;
        [SerializeField] private UnityServicesConfig unityServicesConfig;
        
        [Inject] private IAchievementManager _achievementManager;
        [Inject] private ICustomization _customization;

        public void RunAds()
        {
            _adsService.LoadInterstitial();
            _adsService.ShowInterstitial();
        }

        public void RunBanner()
        {
            _adsService.LoadBanner("Rewarded_Android");
            _adsService.ShowBanner("Rewarded_Android");
        }
        
        #region Unity Life Cycle

        protected override async void Awake()
        {
            try
            {
                base.Awake();
                await UnityAnalytics.InitializeAsync();
            
                await UnityRemoteConfig.InitializeAsync();
                UnityRemoteConfig.FetchCompleted += OnRemoteConfigFetched;
                UnityRemoteConfig.FetchConfigs();
            
                _adsService = new AdsService();
                _adsService.Initialize(unityServicesConfig.GameId, unityServicesConfig.testMode);
            
                // TODO: Uncomment the following lines if you want to enable accelerometer and gyroscope input
                // if (Accelerometer.current == null)
                //     InputSystem.AddDevice<Accelerometer>();
                // InputSystem.EnableDevice(Accelerometer.current);
                // if(Gyroscope.current == null)
                //     InputSystem.AddDevice<Gyroscope>();
                // InputSystem.EnableDevice(Gyroscope.current);
                // foreach (var d in InputSystem.devices)
                //     Debug.Log($"Device: {d}");
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Failed to initialize Unity Services: {e.Message}", e); 
            }
        }

        private void OnEnable()
        {
            RegisterLevelEvents();
            RegisterScoreEvents();
            RegisterAchievementEvents();
            RegisterGameOverEvents();
            RegisterCustomizationEvents();
        }

        private void OnDisable()
        {
            DeregisterLevelEvents();
            DeregisterScoreEvents();
            DeregisterAchievementEvents();
            DeregisterGameOverEvents();
            DeregisterCustomizationEvents();
                        
            // InputSystem.DisableDevice(Accelerometer.current);
            // InputSystem.DisableDevice(Gyroscope.current);
        }

        private void OnDestroy()
        {
            UnityRemoteConfig.FetchCompleted -= OnRemoteConfigFetched;
        }

        #endregion
        
        #region LevelStart

        private void RegisterLevelEvents()
        {
            _levelEvent = new EventBinding<LevelReached>(OnLevelReached);
            EventBus<LevelReached>.Register(_levelEvent);
        }

        private void DeregisterLevelEvents()
        {
            EventBus<LevelReached>.Deregister(_levelEvent);
        }

        private void OnLevelReached(LevelReached e)
        {
            _currentLevel = e.LevelNumber;
            UnityAnalytics.LogLevelStart(e.LevelNumber);
            
            // Ship Part Choices
            if (_currentLevel > 0 && _partSelectionCounts.Count > 0)
            {
                var dict = new Dictionary<string, object>();
                foreach (var kv in _partSelectionCounts)
                    dict[kv.Key.ToString()] = kv.Value;

                UnityAnalytics.LogShipPartChoice(dict);
                _partSelectionCounts.Clear();
            }
        }

        #endregion

        #region ScoreTracking

        private void RegisterScoreEvents()
        {
            _scoreEvent = new EventBinding<ScoreUpdated>(OnScoreUpdated);
            EventBus<ScoreUpdated>.Register(_scoreEvent);
        }

        private void DeregisterScoreEvents()
        {
            EventBus<ScoreUpdated>.Deregister(_scoreEvent);
        }

        private void OnScoreUpdated(ScoreUpdated e)
        {
            _lastScore = (int)e.TotalScore;
        }

        #endregion

        #region AchievementUnlocked

        private void RegisterAchievementEvents()
        {
            if (_achievementManager != null)
                _achievementManager.OnShowAchievement += OnShowAchievement;
        }

        private void DeregisterAchievementEvents()
        {
            if (_achievementManager != null)
                _achievementManager.OnShowAchievement -= OnShowAchievement;
        }

        private void OnShowAchievement(AchievementSO def)
        {
            UnityAnalytics.LogAchievementUnlocked(def.Title);
        }

        #endregion

        #region GameOver

        private void RegisterGameOverEvents()
        {
            _gameOverEvent = new EventBinding<GameStateChangedEvent>(OnGameOver);
            EventBus<GameStateChangedEvent>.Register(_gameOverEvent);
        }

        private void DeregisterGameOverEvents()
        {
            EventBus<GameStateChangedEvent>.Deregister(_gameOverEvent);
        }

        private void OnGameOver(GameStateChangedEvent e)
        {
            if (e.NewState != GameState.gameOver) return;

            bool gotHigh = _achievementManager?.DidHitNewHighThisSession ?? false;
            UnityAnalytics.LogGameOver(_lastScore, _currentLevel, gotHigh);
            
            // Ship Part Choices
            if (_partSelectionCounts.Count > 0)
            {
                var dict = new Dictionary<string, object>();
                foreach (var kv in _partSelectionCounts)
                    dict[kv.Key.ToString()] = kv.Value;

                UnityAnalytics.LogShipPartChoice(dict);

                _partSelectionCounts.Clear();
            }
        }

        #endregion

        #region ShipPartChoice

        private void RegisterCustomizationEvents()
        {
            _customization.OnApplyShipPart += HandleApplyShipPart;
        }

        private void DeregisterCustomizationEvents()
        {
            _customization.OnApplyShipPart -= HandleApplyShipPart;
        }
        
        private void HandleApplyShipPart(PartType type, Transform _, ShipPartSO.StatModifier[] __)
        {
            if (!_partSelectionCounts.ContainsKey(type))
                _partSelectionCounts[type] = 0;
            _partSelectionCounts[type]++;
        }

        #endregion

        #region RemoteConfig

        private void OnRemoteConfigFetched(ConfigResponse resp)
        {
            string json = RemoteConfigService.Instance.appConfig
                .GetJson("levelSpawnRules");

            var wrapper = JsonUtility.FromJson<LevelSpawnRuleArrayWrapper>(
                $"{{\"items\":{json}}}");

            // 3) overwrite your SO’s data
            asteroidsConfig.SetLevelSpawnRules(wrapper.items);

            Debug.Log($"[RC] Applied {wrapper.items.Length} spawn‐rules from remote");
        }
        
        [Serializable]
        private class LevelSpawnRuleArrayWrapper
        {
            public AsteroidsConfig.LevelSpawnRule[] items;
        }

        #endregion
    }
}
